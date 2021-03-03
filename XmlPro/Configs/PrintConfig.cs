using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace XmlPro.Configs
{
    public record PrintConfig
    {
        public static int? DefaultPrintAsLevel = null;
        public static int DefaultLevelsToShow = 3;

        public static bool DefaultShowDeclarative = true;
        public static bool DefaultShowTexts = true;
        public static bool DefaultTrimTextContent = true;
        public static bool DefaultShowElements = true;
        public static bool DefaultEncodeText = false;

        public static bool DefaultAttributesOrderByName = true;
        public static bool DefaultEncodeAttributeName = false;
        public static bool DefaultEncodeAttributeValue = false;
        public static string DefaultUnitIndent = new string(' ', 2);
        public static string DefaultTextConnector = "\n";


        public static Predicate<int> NoMoreThan(int maxLevel) => (int level) => level <= maxLevel;

        private static readonly Dictionary<(string, int), string> CachedIndents = new Dictionary<(string, int), string>();
        public static string IndentOf(int times, string unit=null)
        {
            unit ??= DefaultUnitIndent;
            if (!CachedIndents.ContainsKey((unit, times)))
            {
                string indent = string.Concat(Enumerable.Repeat(unit, times));
                CachedIndents.Add((unit, times), indent);
            }

            return CachedIndents[(unit, times)];
        }

        /// <summary>
        /// Specify Level to print with corresponding indent.
        /// </summary>
        public int? PrintAsLevel { get; init; } = DefaultPrintAsLevel;

        /// <summary>
        /// The maximum levels of node to be printed.
        /// </summary>
        public int MaxLevelToShow { get; init; } = DefaultLevelsToShow;

        /// <summary>
        /// Empty string leading inserted as a single indent.
        /// </summary>
        public string UnitIndent { get; init; } = DefaultUnitIndent;

        /// <summary>
        /// Predicate of any given level to be shown or not.
        /// </summary>
        public Predicate<int> ShowLevel { get; init; } = null;

        /// <summary>
        /// Display Attributes in order of their names if True, otherwise keep the original order in opening tags.
        /// </summary>
        public bool AttributesOrderByName { get; init; } = DefaultAttributesOrderByName;

        /// <summary>
        /// True to show the details of the Declarative nodes (Remark, CDATA, DocType and Declaration), otherwise shown with placeholders.
        /// </summary>
        public bool ShowDeclarative { get; init; } = DefaultShowDeclarative;

        /// <summary>
        /// True to show the XText objects at their presenting positions, False to hide texts.
        /// </summary>
        public bool ShowTexts { get; init; } = DefaultShowTexts;

        /// <summary>
        /// Indicate if each <c>ITextOnly</c> shall be trimmed before showing.
        /// </summary>
        public bool TrimTextContent { get; init; } = DefaultTrimTextContent;

        /// <summary>
        /// True to include contained XElements as indented strings.
        /// </summary>
        public bool ShowElements { get; init; } = DefaultShowElements;

        /// <summary>
        /// True to encode the text contents, otherwise show decoded texts that could make output as invalid XML.
        /// </summary>
        public bool EncodeContent { get; init; } = DefaultEncodeText;

        /// <summary>
        /// True to encode the attribute names, otherwise show decoded texts that could make output as invalid XML.
        /// </summary>
        public bool EncodeAttributeName { get; init; } = DefaultEncodeAttributeName;

        /// <summary>
        /// True to encode the attribute values, otherwise show decoded texts that could make output as invalid XML.
        /// </summary>
        public bool EncodeAttributeValue { get; init; } = DefaultEncodeAttributeValue;

        /// <summary>
        /// Connector of children nodes.
        /// </summary>
        public string TextConnector { get; init; } = DefaultTextConnector;

    }
}
