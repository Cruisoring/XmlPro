using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPro.Enums;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    /// <summary>
    /// Immutable record to represent a XML text node with <c>Level</c> and <c>IScope</c> fixed.
    /// </summary>
    public record XText: StringScope, IContained
    {
        public static readonly PrintConfig DefaultTextConfig = new PrintConfig()
        {
            PrintAsLevel = 0,
            EncodeText = false,
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
        /// Level of the current <c>IContained</c> node relative to the root, it shall be <c>Level</c> of <c>Parent</c> plus one.
        /// </summary>
        public int Level { get; init; }

        public XText(char[] context, int level, int begin, int end) : base(context, begin, end)
        {
            Level = level;
        }

        public string Print(PrintConfig config = null)
        {
            config ??= DefaultTextConfig;
            string text = config.EncodeText ? Encode(Text) : Text;
            return $"{new string(ToStringIndentChar, (config.PrintAsLevel ?? 0) * ToStringIndentMultiplier)}{text}";
        }

        public override string ToString() => Text;
    }
}
