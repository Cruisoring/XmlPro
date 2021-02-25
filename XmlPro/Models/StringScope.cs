using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using XmlPro.Extensions;
using XmlPro.Helpers;
using XmlPro.Interfaces;

namespace XmlPro.Models
{
    public record StringScope : Scope, IText
    {
        public char[] Source { get; init; }

        protected readonly Cache<string> ValueCache;

        public string RawText => ValueCache.Value;

        public StringScope([NotNull] char[] source, int begin, int end) : base(begin, end)
        {
            Source = source;
            ValueCache = new Cache<string>(GetText);
        }

        protected string GetText() => this.TextFrom(Source);

        public override string ToString() => $"[{Begin}, {End}): {RawText}";

    }
}
