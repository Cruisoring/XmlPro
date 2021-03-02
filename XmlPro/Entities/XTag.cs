﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XmlPro.Enums;
using XmlPro.Extensions;
using XmlPro.Helpers;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record XTag : StringScope
    {
        public const string XML_DECALRE = "<?xml";
        public const string DELCARE_END = "?>";
        public const string REMARK_START = "<!--";
        public const string REMARK_END = "-->";
        public const string DocType_START = "<!DOCTYPE";
        public const string DocType_END = ">";
        public const string CDATA_START = "<![CDATA[";
        public const string CDATA_END = "]]>";

        public const char TagBegin = '<';

        public const char TagEnd = '>';
        public const char TagClosing = '/';
        public const char DeclarationIdentifier = '?';
        public const char RemarkIdentifier = '!';
        public const char UnderScore = '_';
        public const char DeclarativeNamePrefix = '#';
        public static readonly char[] NameEndings = new[] {' ', '\t', '\r', '\n', TagClosing, TagEnd};

        public static readonly TagType[] DeclarativeTypes = new[]
            {TagType.Declaration, TagType.CDATA, TagType.DocType, TagType.Remark};

        public static readonly Dictionary<TagType, (string, string)> TagFormats =
            new Dictionary<TagType, (string, string)>()
            {
                {TagType.Declaration, ("<?", DELCARE_END)},
                {TagType.Remark, (REMARK_START, REMARK_END)},
                {TagType.DocType, (DocType_START, DocType_END)},
                {TagType.CDATA, (CDATA_START, CDATA_END)},

                {TagType.Opening, ("<", ">")},
                {TagType.Closing, ("</", ">")},
                {TagType.Sound, ("<", " />")},
            };

        /// <summary>
        /// This PrintConfig shall serialize the content of the XTag as a friendly but also could be illegal XML string.
        /// </summary>
        public static readonly PrintConfig DefaultTagConfig = new PrintConfig()
        {
            AttributesOrderByName = true,
            EncodeText = false,
            EncodeAttributeName = false,
            EncodeAttributeValue = false
        };


        public static IEnumerable<XTag> ParseTags([NotNull] char[] context, int since, int? until = null)
        {
            var (tagType, tagBegin, tagEnd, closingPos, closing, name) = 
                (TagType.Unknown, -1, -1, -1, ">", "");

            int last = until ?? context.Length;
            for (int i = since; i < last; i++)
            {
                char current = context[i];
                if (current == TagBegin)
                {
                    tagBegin = i;

                    char next = context[i + 1];
                    IScope scope = new Scope(i, i+9);
                    int nameEnding = -1;
                    switch (next)
                    {
                        case DeclarationIdentifier:
                            // Declaration tag shall be <?xml ... ?>
                            if (scope.TextFrom(context).StartsWith(XML_DECALRE))
                            {
                                tagType = TagType.Declaration;
                                closing = DELCARE_END;
                                nameEnding = Indexer.IndexOfAny(context, NameEndings, i + 5);
                                name = nameEnding < 0 ? "#xml" : new string(context, i + 2, nameEnding - i - 2);
                            }
                            break;
                        case RemarkIdentifier:
                            // Check if tag is Remark of <!-- ... -->
                            scope = new Scope(i, i + 9);
                            string leading = scope.TextFrom(context);
                            if (leading.StartsWith(REMARK_START))
                            {
                                tagType = TagType.Remark;
                                closing = REMARK_END;
                                name = $"{DeclarativeNamePrefix}comment";
                                nameEnding = i + REMARK_START.Length;
                            }
                            else if (leading.StartsWith(CDATA_START))
                            {
                                tagType = TagType.CDATA;
                                closing = CDATA_END;
                                name = $"{DeclarativeNamePrefix}cdata";
                                nameEnding = i + CDATA_START.Length;
                            }
                            else if (leading.StartsWith(DocType_START))
                            {
                                tagType = TagType.DocType;
                                name = $"{DeclarativeNamePrefix}doctype";
                                nameEnding = i + DocType_START.Length;
                            }
                            else
                            {
                                throw new FormatException(
                                    $"Invalid tag: {scope.TextFrom(context)}...");
                            }
                            break;
                        case TagClosing:
                            tagType = TagType.Closing;
                            nameEnding = Indexer.IndexOfAny(context, NameEndings, i + 2);
                            name = new string(context, i + 2, nameEnding - i - 2);
                            break;
                        default:
                            if (Char.IsLetter(next) || next == UnderScore)
                            {
                                tagType = TagType.Opening;
                                nameEnding = Indexer.IndexOfAny(context, NameEndings, i + 1);
                                name = new string(context, i + 1, nameEnding - i - 1);
                            }
                            else
                            {
                                throw new FormatException($"tag name expected after <: {scope.TextFrom(context)}");
                            }
                            break;
                    }

                    closingPos = Indexer.IndexOf(context, closing.ToCharArray(), nameEnding);
                    if (closingPos == -1)
                    {
                        throw new FormatException(
                            $"XML {tagType} missing '{closing}': {new string(context, tagBegin, last - tagBegin)}");
                    } 
                    else if (tagType == TagType.Opening && context[closingPos - 1] == TagClosing)
                    {
                        tagType = TagType.Sound;
                    }
                    else if (tagType == TagType.DocType)
                    {
                        if (Array.IndexOf(context, '[', tagBegin, closingPos-tagBegin) != -1)
                        {
                            closing = "]>";
                            closingPos = Indexer.IndexOf(context, closing.ToCharArray(), nameEnding);
                        }
                    }

                    tagEnd = closingPos + closing.Length;
                    if (name.StartsWith(DeclarativeNamePrefix))
                    {
                        // Skip extracting attributes from DocType, CDATA and Remark nodes
                        yield return new XTag(context, tagBegin, tagEnd, tagType, name);
                    }
                    else
                    {
                        yield return new XTag(context, tagBegin, tagEnd, tagType, name, XAttribute.ParseAttributes(context, nameEnding, tagEnd));

                    }

                    // index point to one position ahead since it would be increase by one in next loop
                    i = tagEnd-1;
                    // reset local variables
                    (tagType, tagBegin, tagEnd, closingPos, closing, name) =
                        (TagType.Unknown, -1, -1, -1, ">", "");
                }
                else if (WhiteSpaces.Contains(current))
                {
                    // skip white spaces before tags
                    continue;
                }
            }
        }

        public TagType Type { get; init; }
        public string Name { get; init; }
        public XAttribute[] Attributes { get; init; }

        protected XTag(char[] context, int begin, int end) : base(context, begin, end)
        {
        }

        public XTag(char[] context, int begin, int end, TagType tagType, string name=null, IEnumerable<XAttribute> attributes = null) : this(context, begin, end)
        {
            Type = tagType;
            Name = Decode(name);
            Attributes = attributes?.ToArray() ?? new XAttribute[0];
        }

        public override string ToString()
        {
            return Print();
        }

        public string Print(PrintConfig config = null)
        {
            config ??= DefaultTagConfig;
            var (start, end) = TagFormats[Type];
            string name = config.EncodeAttributeName ? Encode(Name) : Name;

            if (DeclarativeTypes.Contains(Type))
            {
                if (config.ShowDeclarative)
                {
                    return Decode(RawText);
                }
                else
                {
                    return $"{start}{name}{end}";
                }
            }
            if (Attributes.Length == 0)
            {
                return $"{start}{name}{end}";
            }

            XAttribute[] attributes = Attributes;
            if (config.AttributesOrderByName)
            {
                attributes = Attributes.ToArray();
                Array.Sort(attributes, attributes.Select(a => a.Name).ToArray());
            }

            StringBuilder sb = new StringBuilder($"{start}{name}");
            foreach (var attribute in attributes)
            {
                sb.Append(" " + (config.EncodeAttributeName ? Encode(attribute.Name) : attribute.Name));
                sb.Append(attribute.Value == null
                    ? ""
                    : "=\"" + (config.EncodeAttributeValue ? Encode(attribute.Value) : attribute.Value) + '"');
            }

            sb.Append(end);
            return sb.ToString();
        }
    }
}
