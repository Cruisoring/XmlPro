using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Web;
using XmlPro.Extensions;
using XmlPro.Helpers;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record StringScope : Scope, IWithText
    {
        public static readonly HashSet<char> WhiteSpaces = new HashSet<char>() {' ', '\n', '\r', '\t'};

        public static string Decode(string raw)
        {
            return HttpUtility.HtmlDecode(raw);
        }

        public static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value);
        }


        public char[] Context { get; init; }

        // protected readonly Cache<string> ValueCache;

        public string RawOuterText => this.TextFrom(Context);

        public virtual string OuterText => Decode(RawOuterText);

        public StringScope([NotNull] char[] context, int begin, int end) : base(begin, end)
        {
            Context = context;
            // ValueCache = new Cache<string>(GetText);
        }

        // protected string GetText() => this.TextFrom(Context);

        public override string ToString() => $"[{Begin}, {End}): {RawOuterText}";

    }
}
