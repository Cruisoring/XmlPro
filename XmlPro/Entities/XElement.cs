using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using XmlPro.Configs;
using XmlPro.Enums;
using XmlPro.Extensions;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record XElement: StringScope, IElement
    {
        public static bool AttributeBestMatch = true;

        /// <summary>
        /// The PrintConfig shall serialize the content of the XDocument as a legal XML string.
        /// </summary>
        public static readonly PrintConfig DefaultElementConfig = new PrintConfig()
        {
            PrintAsLevel = 0,
            MaxLevelToShow = 1,

            ShowDeclarative = false,
            ShowTexts = true,
            ShowElements = true,
            AttributesOrderByName = true,
            EncodeContent = false,
            EncodeAttributeName = false,
            EncodeAttributeValue = false
        };

        public static IEnumerable<IScope> Generate([NotNull] char[] context, ParseConfig config = null)
        {
            config ??= new ParseConfig();
            //Get all settings for parsing
            (int level, int tagEnd, int until, bool preserveWhitespace, bool trimText) = (
                config.StartLevel,
                config.Scope?.Begin ?? 0, 
                config.Scope?.End ?? context.Length,
                config.PreserveWhitespace,
                config.TrimTextNodes);

            IEnumerable<XTag> tags = XTag.Generate(context, config);
            foreach (var tag in tags)
            {
                if (tag.Begin > tagEnd + 1)
                {
                    string text = new string(context, tagEnd, tag.Begin - tagEnd);
                    string trimmed = text.Trim();
                    if (preserveWhitespace || trimmed.Length > 0)
                    {
                        yield return new XTextBlock(
                            context, 
                            level, 
                            trimText ? trimmed : text, //Keep content as trimmed if trimText is TRUE
                            tagEnd, 
                            tag.Begin);
                    }
                }
                switch (tag.Type)
                {
                    case TagType.Sound:
                    case TagType.Declaration:
                    case TagType.CDATA:
                    case TagType.DocType:
                    case TagType.Remark:
                        yield return new XElement(context, tag, level);
                        break;
                    case TagType.Opening:
                        yield return tag;
                        level++;
                        break;
                    case TagType.Closing:
                        yield return tag;
                        level--;
                        break;
                    default:
                        throw new ArgumentException($"Unexpected tag of type {tag.Type}");
                }

                tagEnd = tag.End;
            }
        }


        public static IList<IContained> Conclude([NotNull] char[] context, ParseConfig config = null)
        {
            // Trim Whitespace as default parsing option
            config ??= XDocument.DefaultHtmlParseConfig;

            //Get all settings and local variables for parsing
            (int level, IContainer parent, IList<IElement> elements, IList<ITextOnly> texts) = (
                config.StartLevel,
                config.Root,
                new List<IElement>(),
                null);

            Stack<XTag> unpaired = new Stack<XTag>();
            Stack<(IContainer, IList<IElement>, IList<ITextOnly>)> stack = new Stack<(IContainer, IList<IElement>, IList<ITextOnly>)>();

            IEnumerable<IScope> parts = Generate(context, config);
            foreach (var part in parts)
            {
                if (part is IElement element)
                {
                    element.Parent = parent;
                    elements.Add(element);
                }
                else if (part is XTextBlock text)
                {
                    if (texts == null)
                    {
                        texts = new List<ITextOnly>();
                    }
                    text.Parent = parent;
                    texts.Add(text);
                }
                else if (part is XTag tag)
                {
                    if (tag.Type == TagType.Opening)
                    {
                        unpaired.Push(tag);
                        level++;
                        stack.Push((parent, elements, texts));
                        (parent, elements, texts) = (null, new List<IElement>(), null);
                    }
                    else if (tag.Type == TagType.Closing)
                    {
                        var opening = unpaired.Pop();
                        level--;
                        if (opening.Name != tag.Name)
                        {
                            throw new FormatException($"Elements are not properly nested: </{tag.Name}> cannot pair with <{opening.Name}>");
                        }

                        var newElement = new XElement(context, opening, tag, level, elements, texts);
                        (parent, elements, texts) = stack.Pop();
                        newElement.Parent = parent;
                        elements.Add(newElement);
                    }
                    else
                    {
                        throw new ArgumentException($"Unexpected tag of type {tag.Type}");
                    }
                }
                else
                {
                    throw new NotImplementedException($"Unhandled part: {part}");
                }
            }

            if (unpaired.Count > 0)
            {
                throw new ArgumentException("Failed to pair all tags.");
            }
            else if (texts == null)
            {
                return elements.Cast<IContained>().ToList();
            }
            else
            {
                List<IContained> nodes = new List<IContained>();
                nodes.AddRange(elements);
                nodes.AddRange(texts);
                nodes.Sort();
                return nodes;
            }
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
        public IList<ITextOnly> Texts { get; init; }

        protected IContained[] Nodes;

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
            Nodes = null;
        }

        public XElement([NotNull] char[] context, [NotNull] XTag opening, [NotNull] XTag closing, int level,
            IList<IElement> elements = null, IList<ITextOnly> texts = null) : 
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
                Nodes = elements?.Cast<IContained>().ToArray();
            }
            else if (elements == null)
            {
                Nodes = texts?.Cast<IContained>().ToArray();
            }
            else
            {
                Nodes = elements.Cast<IContained>().Union(texts.Cast<IContained>()).ToArray();
                Array.Sort(Nodes);
            }

            // Set all children' parent to this
            Nodes?.ForEach(node => node.Parent = this);
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

        public IEnumerable<IElement> this[Predicate<IElement> predicate, bool recursively = true]
        {
            get
            {
                if (predicate(this))
                {
                    yield return this;
                }
                if (recursively && Elements != null)
                {
                    foreach (var element in Elements)
                    {
                        if (predicate(element))
                        {
                            yield return element;
                        }
                        else if (element is IContainer container)
                        {
                            foreach (var contained in container[predicate, true])
                            {
                                yield return contained;
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<IWithText> SearchText(Predicate<IWithText> predicate, bool resusively = true)
        {
            foreach (var node in Nodes)
            {
                if (resusively && node is XElement element)
                {
                    foreach (var matchedChildText in element.SearchText(predicate, resusively))
                    {
                        yield return matchedChildText;
                    }
                }
                else if (resusively && node is XTextBlock text)
                {
                    if (predicate(text))
                    {
                        yield return text;
                    }
                }
            }
        }

        public IEnumerable<IWithText> SearchText(string keyword)
        {
            return SearchText(node => node.OuterText.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        public string GetText(int? index = null, char connector = '\n')
        {
            return ScopeExtensions.GetText(Texts, index);
        }

        public IEnumerable<string> AsStrings([NotNull] PrintConfig config)
        {
            var (maxLevel, unitIndent, childConnector, showLevel,
                    attrOrderByName, showDeclarative, showTexts, showElements, trimText,
                    encodeContent, encodeAttrName, encodeAttrValue) =
                (
                    config.MaxLevelToShow, config.UnitIndent, config.TextConnector,
                    config.ShowLevel ?? PrintConfig.NoMoreThan(config.MaxLevelToShow),
                    config.AttributesOrderByName,
                    config.ShowDeclarative,
                    config.ShowTexts,
                    config.ShowElements,
                    config.TrimTextContent,
                    config.EncodeContent,
                    config.EncodeAttributeName,
                    config.EncodeAttributeValue);

            bool showThisLevel = showLevel(Level);
            string currentIndent = PrintConfig.IndentOf(Level, unitIndent);

            string opening = Opening.AsString(attrOrderByName, encodeAttrName, encodeAttrValue);
            string closing = Closing?.AsString(attrOrderByName, encodeAttrName, encodeAttrValue) ?? "";
            // Stop quickly with simple Elements or Level >= maxLevel as specified in PrintConfig.LevelsToShow
            if (Level == maxLevel || Type != ElementType.Compound || !showElements)
            {
                if (showThisLevel && (showDeclarative || Type != ElementType.Declarative))
                {
                    yield return $"{currentIndent}{opening}{closing}";
                }
                yield break;
            }
            else if (Level > maxLevel)
            {
                yield break;
            }

            //Now Level < maxLevel && Type == ElementType.Compound
            string childIndent = currentIndent + unitIndent;
            bool showNextLevel = showLevel(Level + 1);
            // Try to show element as a single line with/without text and break
            if (Elements == null || Elements.Count == 0)
            {
                if (!showTexts || !showNextLevel || Texts == null || Texts.Count == 0)
                {
                    if (showThisLevel)
                    {
                        yield return $"{currentIndent}{opening}{closing}";
                    }
                    yield break;
                }
                else if (Texts.Count == 1)
                {
                    string content = encodeContent ? Encode(Texts[0].Content) : Texts[0].Content;
                    content = trimText ? content.Trim() : content;
                    yield return showThisLevel ? $"{currentIndent}{opening}{content}{closing}" : $"{childIndent}{content}";
                    yield break;
                }
            }

            //Since no yield break, show element with indented texts/elements
            if (showThisLevel)
            {
                yield return $"{currentIndent}{opening}";
            }
            foreach (var node in Nodes)
            {
                if (node is ITextOnly text)
                {
                    if (showTexts && showNextLevel)
                    {
                        string content = encodeContent ? Encode(Texts[0].Content) : Texts[0].Content;
                        content = trimText ? content.Trim() : content;
                        yield return $"{childIndent}{content}";
                    }
                }
                else if (node is IElement element)
                {
                    if (showElements)
                    {
                        IEnumerable<string> lines = element.AsStrings(config);
                        foreach (var line in lines)
                        {
                            yield return line;
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException("Unexpected scenario happened.");
                }
            }
            if (showThisLevel)
            {
                yield return $"{currentIndent}{closing}";
            }
        }

        public string ToStringWith(PrintConfig config = null)
        {
            config ??= DefaultElementConfig;
            var lines = AsStrings(config);
            string result = string.Join(config.TextConnector, lines);
            return result;
        }

        public override string ToString()
        {
            return ToStringWith();
        }
    }
}
