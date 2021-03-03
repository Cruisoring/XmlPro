using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPro.Interfaces;

namespace XmlPro.Configs
{
    public record ParseConfig
    {
        public static int DefaultStartLevel = 0;
        public static bool DefaultPreserveWhitespace = false;
        public static bool DefaultTrimTextNodes = true;

        public IContainer Root { get; init; } = null;

        /// <summary>
        /// Level of the first IElement to be parsed.
        /// </summary>
        public int StartLevel { get; init; } = DefaultStartLevel;

        /// <summary>
        /// Specify the range of text/chars to be parsed. NULL to allow context-based selections.
        /// </summary>
        public IScope Scope { get; init; } = null;

        /// <summary>
        /// Preserve ALL whitespaces between ANY tags if <c>TRUE</c>, otherwise ignore empty text blocks if <c>FALSE</c>.
        /// </summary>
        public bool PreserveWhitespace { get; init; } = DefaultPreserveWhitespace;

        /// <summary>
        /// Only applied when <c>PreserveWhitespace</c> is set to <c>TRUE</c> to trim leading/ending WhiteSpaces for easier data processing.
        /// </summary>
        public bool TrimTextNodes { get; init; } = DefaultTrimTextNodes;
    }
}
