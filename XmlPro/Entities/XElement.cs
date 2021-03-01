using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using XmlPro.Enums;
using XmlPro.Extensions;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record XElement: StringScope, IElement
    {
        public static bool SkipEmptyText = true;
        public static bool AttributeBestMatch = true;
        public static bool DefaultIncludingChildren = false;

        public static IEnumerable<IContained> Parse([NotNull] char[] context, int since, int? until = null, IContainer parent=null)
        {
            Stack<XTag> unpaired = new Stack<XTag>();
            IList<IText> texts = null;
            IList<IElement> children = new List<IElement>();
            Stack<(IList<IElement>, IList<IText>)> stack = new Stack<(IList<IElement>, IList<IText>)>();
            int level = (parent?.Level ?? 0) + 1;

            int lastEnd = since;
            IEnumerable<XTag> tags = XTag.ParseTags(context, since, until);
            foreach (var tag in tags)
            {
                if (tag.Begin > lastEnd+1)
                {
                    string text = new string(context, lastEnd, tag.Begin-lastEnd);
                    if (!SkipEmptyText || text.Trim().Length > 0)
                    {
                        if (texts == null)
                        {
                            texts = new List<IText>();
                        }
                        texts.Add(new XText(context, level, lastEnd, tag.Begin));
                    }
                }
                switch (tag.Type)
                {
                    case TagType.Sound:
                    case TagType.Declaration:
                    case TagType.CDATA:
                    case TagType.DocType:
                    case TagType.Remark:
                        var child = new XElement(context, tag, level);
                        children.Add(child);
                        break;
                    case TagType.Opening:
                        unpaired.Push(tag);
                        stack.Push((children, texts));
                        (children, texts) = (new List<IElement>(), null);
                        level++;
                        break;
                    case TagType.Closing:
                        var opening = unpaired.Pop();
                        level--;
                        if (opening == null)
                        {
                            throw new FormatException($"Missing opening tag <{tag.Name}>");
                        }
                        else if (opening.Name != tag.Name)
                        {
                            throw new FormatException($"Elements are not properly nested: </{tag.Name}> cannot pair with <{opening.Name}>");
                        }
                        else
                        {
                            var newElement = new XElement(context, opening, tag, level, children, texts);
                            (children, texts) = stack.Pop();
                            children.Add(newElement);
                        }
                        break;
                    default:
                        throw new ArgumentException($"Unexpected tag of type {tag.Type}");
                }

                lastEnd = tag.End;
            }

            if (unpaired.Count > 0)
            {
                throw new ArgumentException("Failed to pair all tags.");
            }

            return children;
        }

        /// <summary>
        /// The container of this node, could be updated by the closing tag of the <c>Parent</c> node.
        /// </summary>
        public IContainer Parent { get; set; }

        /// <summary>
        /// Level of the current <c>IContained</c> node relative to the root, it shall be <c>Level</c> of <c>Parent</c> plus one.
        /// Can be used as filter or by <code>ToStringWithConfig()</code>.
        /// </summary>
        public int Level { get; init; }

        /// <summary>
        /// Type of this node.
        /// </summary>
        public ElementType Type { get; init; }

        public string Name { get; init; }
        public XTag Opening { get; init; }
        public XTag Closing { get; init; }

        public IList<IElement> Children { get; init; }
        public IList<IText> Texts { get; init; }

        public Dictionary<string, string> Attributes { get; init; }

        public XElement([NotNull] char[] context, [NotNull] XTag opening, int level) : base(context, opening.Begin, opening.End)
        {
            Opening = opening;
            Level = level;
            switch (opening.Type)
            {
                case TagType.Sound:
                    Type = ElementType.Simple;
                    break;
                case TagType.Declaration:
                case TagType.CDATA:
                case TagType.DocType:
                case TagType.Remark:
                    Type = ElementType.Declarative;
                    break;
                default:
                    throw new FormatException($"Invalid {opening.Type} tag to compose a sound XElement.");
            }

            Name = Opening.Name;
            Attributes = Opening.Attributes.ToDictionary(
                attr => attr.Name,
                attr => attr.Value
            );
            Children = null;
            Texts = null;
        }

        public XElement([NotNull] char[] context, [NotNull] XTag opening, [NotNull] XTag closing, int level,
            IList<IElement> children = null, IList<IText> texts = null) : 
            base(context, opening.Begin, closing.End)
        {
            Opening = opening;
            Closing = closing;
            if (Opening.Type != TagType.Opening || Closing.Type != TagType.Closing)
            {
                throw new ArgumentException($"Compound element must be enclosed by opening and closing tags.");
            }
            else if (Opening.Name != Closing.Name)
            {
                throw new ArgumentException($"Opening.Name '{opening.Name}' != '{closing.Name}' Closing.Name");
            }

            Type = ElementType.Compound;
            Level = level;
            Name = Opening.Name;
            Attributes = Opening.Attributes.ToDictionary(
                attr => attr.Name,
                attr => attr.Value
            );

            Children = children;
            Children.ForEach(c => c.Parent = this);
            Texts = texts;
        }

        public string this[string attrName]
        {
            get
            {
                if (Attributes.ContainsKey(attrName))
                {
                    return Attributes[attrName];
                }
                else if (AttributeBestMatch)
                {
                    //TODO: more complex matching like ignoring chars like '_', ' '...
                    string bestMatchedNamed =
                        Attributes.Keys.FirstOrDefault(k => k.Equals(attrName, StringComparison.OrdinalIgnoreCase));
                    return bestMatchedNamed == null ? null : Attributes[bestMatchedNamed];
                }
                else
                {
                    return null;
                }
            }
        }

        public IElement this[int childIndex]
        {
            get
            {
                if (Children == null)
                {
                    throw new InvalidOperationException($"{Type} Element has no children.");
                }
                else if (Children.Count == 0 || childIndex >= Children.Count || childIndex < -Children.Count)
                {
                    throw new IndexOutOfRangeException($"Index {childIndex} is out of range with children of {Children.Count}");
                }
                else
                {
                    return Children[childIndex < 0 ? Children.Count + childIndex : childIndex];
                }
            }
        }

        public string GetText(int? index = null)
        {
            return ScopeExtensions.GetText(Texts, index);
        }

        public string ToString(int indentLevel, bool showText=true, bool? includeChildren=null)
        {
            string indent = new string(ToStringIndentChar, indentLevel*ToStringIndentMultiplier);
            if (Type == ElementType.Compound)
            {
                StringBuilder sb = null;
                if (showText && Texts != null)
                {
                    if (Children.Count == 0)
                    {
                        return $"{indent}{Opening}{GetText(null)}{Closing}";
                    }
                    else
                    {
                        sb = new StringBuilder();
                        sb.AppendLine($"{indent}{Opening}");
                        string childrenIndent = new string(ToStringIndentChar, (1+indentLevel) * ToStringIndentMultiplier);
                        IEnumerable<IContained> nodes = Children.Cast<IContained>()
                            .Union(Texts.Cast<IContained>()).OrderBy(node => node.Begin);
                        IContained lastNode = null;
                        foreach (var node in nodes)
                        {
                            if (node is XText text)
                            {
                                sb.Append($"{childrenIndent}{text.Text.TrimStart()}");
                            }
                            else
                            {
                                string nodeString = node.ToString(indentLevel + 1, showText, includeChildren);
                                sb.AppendLine(lastNode is XText ? nodeString.TrimStart() : nodeString);
                            }

                            lastNode = node;
                        }
                        sb.AppendLine($"{indent}{Closing}");
                        return sb.ToString();
                    }
                }
                else if (Children.Count == 0)
                {
                    return $"{indent}{Opening}{Closing}";
                }
                else
                {
                    sb = new StringBuilder();
                    sb.AppendLine($"{indent}{Opening}");
                    if (includeChildren ?? DefaultIncludingChildren)
                    {
                        Children.ForEach(c => sb.AppendLine(c.ToString(indentLevel + 1, showText, includeChildren)));
                    }
                    sb.Append($"{indent}{Closing}");
                    return sb.ToString();
                }
            }
            else
            {
                return $"{indent}{Opening}";
            }
        }

        public override string ToString()
        {
            if (Type == ElementType.Compound)
            {
                return DefaultIncludingChildren ? ToString(0) : $"{Opening}...{Closing}";
            }
            else
            {
                return Opening.ToString();
            }
        }
    }
}
