using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Entities
{
    public record PrintConfig
    {
        public static int? DefaultPrintAsLevel = null;
        public static int DefaultMaxNodeLevelToShow = 3;
        public static string DefaultUnitIndent = null;

        public static bool DefaultShowDeclarative = true;
        public static bool DefaultShowTexts = true;
        public static bool DefaultShowElements = true;
        public static bool DefaultEncodeText = false;

        public static bool DefaultAttributesOrderByName = true;
        public static bool DefaultEncodeAttributeName = false;
        public static bool DefaultEncodeAttributeValue = false;

        public int? PrintAsLevel { get; init; } = DefaultPrintAsLevel;

        /// <summary>
        /// The maximum level of node to be shown.
        /// </summary>
        public int MaxNodeLevelToShow { get; init; } = DefaultMaxNodeLevelToShow;

        public string UnitIndent { get; init; } = DefaultUnitIndent;

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
        /// True to include contained XElements as indented strings.
        /// </summary>
        public bool ShowElements { get; init; } = DefaultShowElements;

        /// <summary>
        /// True to encode the text contents, otherwise show decoded texts that could make output as invalid XML.
        /// </summary>
        public bool EncodeText { get; init; } = DefaultEncodeText;

        /// <summary>
        /// True to encode the attribute names, otherwise show decoded texts that could make output as invalid XML.
        /// </summary>
        public bool EncodeAttributeName { get; init; } = DefaultEncodeAttributeName;

        /// <summary>
        /// True to encode the attribute values, otherwise show decoded texts that could make output as invalid XML.
        /// </summary>
        public bool EncodeAttributeValue { get; init; } = DefaultEncodeAttributeValue;
    }
}
