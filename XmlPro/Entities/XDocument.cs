using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using XmlPro.Configs;
using XmlPro.Enums;
using XmlPro.Extensions;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record XDocument : StringScope, IContainer
    {
        public static readonly ElementType[] RootElementTypes = new[] {ElementType.Compound, ElementType.Simple};

        public static readonly ParseConfig DefaultXmlParseConfig = new ParseConfig()
            {PreserveWhitespace = true, TrimTextNodes = false};

        public static readonly ParseConfig DefaultHtmlParseConfig = new ParseConfig()
            {PreserveWhitespace = false, TrimTextNodes = true};


        /// <summary>
        /// The PrintConfig shall serialize the content of the XDocument as a legal XML string.
        /// </summary>
        public static readonly PrintConfig ShowAllPrintConfig = new PrintConfig()
        {
            PrintAsLevel = 0,
            MaxLevelToShow = 1000,
            AttributesOrderByName = false,
            ShowDeclarative = true,
            ShowTexts = true,
            ShowElements = true,
            EncodeContent = true,
            EncodeAttributeName = true,
            EncodeAttributeValue = true
        };

        public int Level { get; init; }

        public IList<IElement> Elements { get; init; }

        public IList<ITextOnly> Texts { get; }

        public XElement Root { get; init; }

        public XDocument([NotNull] char[] context, [NotNull] IList<IElement> elements, IList<ITextOnly> texts = null) : 
            base(context, 0, context.Length)
        {
            Level = 0;
            Elements = elements;
            Texts = texts;
            elements.ForEach(c => c.Parent = this);

            Root = elements.Single(c => RootElementTypes.Contains(c.Type)) as XElement;
        }

        public string this[string attrName] => Root[attrName];

        public IElement this[int childIndex] => Root[childIndex];

        public IEnumerable<IElement> this[Predicate<IElement> filter, bool recursively=false] =>
            Root[filter, recursively];

        public string GetText(int? index = null, char connector = '\n')
        {
            return ScopeExtensions.GetText(Texts, index, connector);
        }

        public IEnumerable<string> AsStrings([NotNull] PrintConfig config)
        {
            return Root.AsStrings(config ?? ShowAllPrintConfig);
        }

        public string InnerText
        {
            get
            {
                IEnumerable<IContained> allTexts = this[node => node.Type == ElementType.Text, true];
                IEnumerable<string> strings = allTexts.Select(node => $"{PrintConfig.IndentOf(node.Level)}{node.OuterText}");
                return string.Join('\n', strings);
            }
        }
    }
}
