using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using XmlPro.Enums;
using XmlPro.Extensions;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record XElement: StringScope, IElement
    {
        public static bool SkipEmptyText = true;
        public static bool AttributeBestMatch = true;
        public static bool DefaultIncludingElements = false;

        /// <summary>
        /// The PrintConfig shall serialize the content of the XDocument as a legal XML string.
        /// </summary>
        public static readonly PrintConfig DefaultElementConfig = new PrintConfig()
        {
            PrintAsLevel = 0,
            MaxNodeLevelToShow = 4,

            ShowDeclarative = false,
            ShowTexts = true,
            ShowElements = true,

            AttributesOrderByName = true,
            EncodeText = false,
            EncodeAttributeName = false,
            EncodeAttributeValue = false
        };



        public static IEnumerable<IContained> Parse([NotNull] char[] context, int since, int? until = null, IContainer parent=null)
        {
            Stack<XTag> unpaired = new Stack<XTag>();
            IList<IText> texts = null;
            IList<IElement> elements = new List<IElement>();
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
                        var element = new XElement(context, tag, level);
                        elements.Add(element);
                        break;
                    case TagType.Opening:
                        unpaired.Push(tag);
                        stack.Push((elements, texts));
                        (elements, texts) = (new List<IElement>(), null);
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
                            var newElement = new XElement(context, opening, tag, level, elements, texts);
                            (elements, texts) = stack.Pop();
                            elements.Add(newElement);
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

            return elements;
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

        public IList<IElement> Elements { get; init; }
        public IList<IText> Texts { get; init; }

        protected IContained[] nodes;

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
            Elements = null;
            Texts = null;
            nodes = null;
        }

        public XElement([NotNull] char[] context, [NotNull] XTag opening, [NotNull] XTag closing, int level,
            IList<IElement> elements = null, IList<IText> texts = null) : 
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

            Elements = elements;
            Elements.ForEach(c => c.Parent = this);
            Texts = texts;
            if (Texts == null)
            {
                nodes = elements?.Cast<IContained>().ToArray();
            }
            else if (elements == null)
            {
                nodes = texts?.Cast<IContained>().ToArray();
            }
            else
            {
                nodes = elements.Cast<IContained>().Union(texts.Cast<IContained>()).ToArray();
                Array.Sort(nodes);
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

        public IElement this[int childIndex]
        {
            get
            {
                if (Elements == null)
                {
                    throw new InvalidOperationException($"{Type} Element has no elements.");
                }
                else if (Elements.Count == 0 || childIndex >= Elements.Count || childIndex < -Elements.Count)
                {
                    throw new IndexOutOfRangeException($"Index {childIndex} is out of range with elements of {Elements.Count}");
                }
                else
                {
                    return Elements[childIndex < 0 ? Elements.Count + childIndex : childIndex];
                }
            }
        }

        public IEnumerable<IContained> this[Predicate<IContained> filter, bool recursively = false]
        {
            get
            {
                // if (recursively)
                // {
                //     foreach (var node in nodes)
                //     {
                //         
                //     }
                // }
                throw new NotImplementedException();
            }
        }


        public string GetText(int? index = null)
        {
            return ScopeExtensions.GetText(Texts, index);
        }

        public string Print(PrintConfig config = null)
        {
            config ??= DefaultElementConfig;
            int level = config.PrintAsLevel ?? Level;
            var (maxLevel, attrOrdered, showDeclarative, showTexts, showElements, encodeText, encodeAttrName,
                    encodeAttrValue) =
                (config.MaxNodeLevelToShow, config.AttributesOrderByName, config.ShowDeclarative, config.ShowTexts,
                    config.ShowElements, config.EncodeText, config.EncodeAttributeName, config.EncodeAttributeValue);
        
            string indent = new string(ToStringIndentChar, level * ToStringIndentMultiplier);
            if (level > maxLevel)
            {
                // Not print when the level of the node is greater than maxLevel
                return "";
            }
            else if (Type == ElementType.Declarative && !showDeclarative)
            {
                // Not print when this node is Declarative and that is not allowed to print
                return "";
            }
            else if (level == maxLevel || Type != ElementType.Compound)
            {
                string text = Texts == null ? "" : string.Join(' ', Texts.Select(t => t.ToString()));
                // When this node is simple or the max level to show, print its own tags and text
                return $"{indent}{Opening.Print(config)}{text}{Closing?.Print(config)}";
            }
            else
            {
                IList<IContained> children = nodes.Where(node =>
                    config.ShowDeclarative || node.Type != ElementType.Declarative).ToList();

                if (children.Count == 0)
                {
                    return $"{indent}{Opening.Print(config)}{Closing.Print(config)}";
                }

                PrintConfig childConfig = config with {PrintAsLevel = config.PrintAsLevel + 1};
                StringBuilder sb = new StringBuilder($"{indent}{Opening.Print(config)}\n");
                IEnumerable<string> lines = children.Select(node =>
                    (node is IElement element) ? $"{element.Print(childConfig)}\n" : node.Print(childConfig));
                lines.ForEach(l => sb.Append(l));
                if (sb[^1] != '\n')
                {
                    sb.AppendLine();
                }
                sb.AppendLine($"{indent}{Closing.Print(config)}");
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return Print();
        }
    }
}
