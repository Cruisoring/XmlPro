using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace XmlPro.Models
{
    public record XAttribute : StringScope, IComparable<XAttribute>
    {
        public const char NameEnding = '=';
        public const char SingleQuote = '\'';
        public const char DoubleQuote = '"';
        public const char TagEnd = '>';
        public const char TagClosing = '/';

        public static IEnumerable<XAttribute> AttributesWithin([NotNull] char[] context, int since, int? until = null)
        {
            var (nameBegin, nameEnd, equalPosition, valueBegin, valueEnd, valueQuote) =
                (-1, -1, -1, -1, -1, DoubleQuote);

            int last = until ?? context.Length;
            for (int i = since; i < last; i++)
            {
                char current = context[i];
                if (nameBegin < 0)
                {
                    if (current == TagEnd || current == TagClosing)
                    {
                        // End of the tag, return immediately if no Attribute name detected
                        if (nameBegin < 0)
                        {
                            yield break;
                        }
                        else
                        {
                            nameEnd = nameEnd < 0 ? i - 1 : nameEnd;
                            yield return new XAttribute(context, nameBegin, nameEnd, null, null);
                            (nameBegin, nameEnd, equalPosition, valueBegin, valueEnd, valueQuote) =
                                (-1, -1, -1, -1, -1, DoubleQuote);
                        }
                    }
                    else if (WhiteSpaces.Contains(current))
                    {
                        // Skip the white spaces before attribute names
                        continue;
                    }
                    else
                    {
                        nameBegin = i;
                    }
                }
                else if (nameEnd < 0)
                {
                    if (current == NameEnding)
                    {
                        equalPosition = i;
                        nameEnd = i;
                    }
                    else if (WhiteSpaces.Contains(current)) // Out of scope of XML specification?
                    {
                        nameEnd = i;
                    }
                }
                else if (equalPosition < 0)
                {
                    if (current == NameEnding)
                    {
                        equalPosition = i;
                    }
                    else if (WhiteSpaces.Contains(current))
                    {
                        continue;
                    }
                    else // Out of scope of XML specification?
                    {
                        yield return new XAttribute(context, nameBegin, nameEnd, null, null);
                        nameBegin = i;
                        nameEnd = -1;
                    }
                }
                else if (valueBegin < 0)
                {
                    if (current == DoubleQuote)
                    {
                        // Most common case: attribute value starts after DoubleQuote
                        valueBegin = i + 1;
                    }
                    else if (current == SingleQuote)
                    {
                        // Support pair of singleQuotes to enclose the attribute value
                        valueQuote = SingleQuote;
                        valueBegin = i + 1;
                    }
                    else if (current == ' ' || WhiteSpaces.Contains(current))
                    {
                        // Skip the white spaces before attribute value
                        continue;
                    }
                    else
                    {
                        throw new NotImplementedException(
                            $"Unexpected char '{current}' within: {new string(context, nameBegin, i + 1)}");
                    }
                }
                else if (valueEnd < 0)
                {
                    if (current == valueQuote)
                    {
                        // Most common case when attribute value ends before the same singleQuote or doubleQuote
                        valueEnd = i+1;
                        yield return new XAttribute(context, nameBegin, nameEnd, valueBegin, valueEnd);
                        (nameBegin, nameEnd, equalPosition, valueBegin, valueEnd, valueQuote) =
                            (-1, -1, -1, -1, -1, DoubleQuote);
                    }
                }
            }
        }

        public string Name { get; init; }
        public string Value { get; init; }


        public XAttribute(char[] context, int begin, int end) : base(context, begin, end)
        {
        }

        public XAttribute(char[] context, int nameBegin, int nameEnd, int? valueBegin, int? valueEnd)
            : this(context, nameBegin, valueEnd ?? nameEnd)
        {
            var rawName = new string(context, nameBegin, nameEnd - nameBegin);
            Name = Decode(rawName);

            if (valueBegin != null && valueEnd != null)
            {
                var rawValue = new string(context, valueBegin.Value, valueEnd.Value - valueBegin.Value - 1);
                Value = Decode(rawValue);
            }
        }

        public override string ToString()
        {
            return $"{Name}=\"{Value}\"";
        }

        public int CompareTo(XAttribute other)
        {
            return String.Compare(this.Name, other.Name, StringComparison.Ordinal);
        }
    }
}