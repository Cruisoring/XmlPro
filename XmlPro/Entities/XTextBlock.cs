using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPro.Configs;
using XmlPro.Enums;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    /// <summary>
    /// Immutable record to represent a XML text node with <c>Level</c> and <c>IScope</c> fixed.
    /// </summary>
    public record XTextBlock: StringScope, ITextOnly
    {
        public static readonly PrintConfig DefaultTextConfig = new PrintConfig()
        {
            PrintAsLevel = 0,
            EncodeContent = false,
        };

        /// <summary>
        /// The container of this node, either <c>XDocument</c> or <c>XElement</c>.
        /// </summary>
        public IContainer Parent { get; set; }

        /// <summary>
        /// Fixed <c>ElementType.Text</c> Type.
        /// </summary>
        public ElementType Type => ElementType.Text;

        /// <summary>
        /// User-friendly decoded content of this instance.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Level of the current <c>IContained</c> node relative to the root, it shall be <c>Level</c> of <c>Parent</c> plus one.
        /// </summary>
        public int Level { get; init; }

        public XTextBlock(char[] context, int level, string content, int begin, int end) : base(context, begin, end)
        {
            Level = level;
            Content = Decode(content);
        }

        public string Print(PrintConfig config = null)
        {
            config ??= DefaultTextConfig with {PrintAsLevel = Level};
            string text = config.EncodeContent ? Encode(OuterText) : OuterText;
            return $"{PrintConfig.IndentOf(config.PrintAsLevel ?? 0)}{text}";
        }

        public string AsIndented(string indent = "", bool encodeContent = true)
        {
            if (encodeContent)
            {
                return $"{indent}{RawOuterText.TrimStart()}";
            }
            else
            {
                return $"{indent}{Content.TrimStart()}";
            }
        }

        public override string ToString() => Content;
    }
}
