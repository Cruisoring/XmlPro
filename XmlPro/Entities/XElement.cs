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
    public record XElement: StringScope, IContainer, IContained
    {
        public static bool SkipEmptyText = true;
        public static bool AttributeBestMatch = true;
        public static bool ToStringWithChildrenDetails = false;

        public static IEnumerable<IContained> Parse([NotNull] char[] context, int since, int? until = null, IContainer parent=null)
        {
            Stack<XTag> unpaired = new Stack<XTag>();
            List<IContained> children = new List<IContained>();
            Stack<List<IContained>> stack = new Stack<List<IContained>>();

            int lastEnd = since;
            IEnumerable<XTag> tags = XTag.ParseTags(context, since, until);
            foreach (var tag in tags)
            {
                if (tag.Begin > lastEnd)
                {
                    string text = new string(context, lastEnd, tag.Begin-lastEnd);
                    if (!SkipEmptyText || text.Trim().Length > 0)
                    {
                        children.Add(new XText(context, lastEnd, tag.Begin));
                    }
                }
                switch (tag.Type)
                {
                    case TagType.Sound:
                    case TagType.Declaration:
                    case TagType.CDATA:
                    case TagType.DocType:
                    case TagType.Remark:
                        var child = new XElement(context, tag, parent);
                        children.Add(child);
                        break;
                    case TagType.Opening:
                        unpaired.Push(tag);
                        stack.Push(children);
                        children = new List<IContained>();
                        break;
                    case TagType.Closing:
                        var opening = unpaired.Pop();
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
                            var newElement = new XElement(context, opening, tag, children);
                            children = stack.Pop();
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


        public ElementType Type { get; init; }

        public IContainer Parent { get; set; }
        public string Name { get; init; }
        public XTag Opening { get; init; }
        public XTag Closing { get; init; }

        public IList<IContained> Children { get; init; }

        public Dictionary<string, string> Attributes { get; init; }

        public XElement([NotNull] char[] context, [NotNull] XTag opening, IContainer parent=null) : base(context, opening.Begin, opening.End)
        {
            Opening = opening;
            Parent = parent;
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
        }

        public XElement([NotNull] char[] context, [NotNull] XTag opening, [NotNull] XTag closing,
            IList<IContained> children = null) : 
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
            Name = Opening.Name;
            Attributes = Opening.Attributes.ToDictionary(
                attr => attr.Name,
                attr => attr.Value
            );

            Children = children;
            Children.ForEach(c => c.Parent = this);
        }

        public string ToString(int indentLevel, bool? includeChildren=null)
        {
            string indent = new string(ToStringIndentChar, indentLevel*ToStringIndentMultiplier);
            if (Type == ElementType.Compound)
            {
                if (Children.Any(c => c is XElement))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"{indent}{Opening}");
                    if (includeChildren ?? ToStringWithChildrenDetails)
                    {
                        Children.ForEach(c => sb.AppendLine(c.ToString(indentLevel + 1, includeChildren)));
                    }
                    sb.Append($"{indent}{Closing}");
                    return sb.ToString();
                }
                else
                {
                    return $"{indent}{Opening}{String.Join("", Children.Select(e => e.ToString(0, includeChildren)))}{Closing}";
                }
            }
            else
            {
                return $"{indent}{Opening}";
            }
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

        public IContained this[int childIndex]
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

        public override string ToString()
        {
            if (Type == ElementType.Compound)
            {
                return ToStringWithChildrenDetails ? ToString(0) : $"{Opening}...{Closing}";
            }
            else
            {
                return Opening.ToString();
            }
        }
    }
}
