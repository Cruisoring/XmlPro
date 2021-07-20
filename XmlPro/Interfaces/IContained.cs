using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPro.Configs;
using XmlPro.Entities;
using XmlPro.Enums;

namespace XmlPro.Interfaces
{
    /// <summary>
    /// Interface representing XML nodes to be contained by <c>IContainer</c> nodes.
    /// </summary>
    public interface IContained : IScope, IWithText, IWithLevel
    {
        /// <summary>
        /// The container of this node, could be updated by the closing tag of the <c>Parent</c> node.
        /// </summary>
        public IContainer Parent { get; set; }

        /// <summary>
        /// Type of this node.
        /// </summary>
        public ElementType Type { get; }

        /// <summary>
        /// Serialize the <c>IContained</c> into string with options specified in the <c>config</c>.
        /// </summary>
        /// <param name="config">The <c>PrintConfig</c> instance instructing how to serialize.</param>
        /// <returns>The serialized string of the <c>IContained</c> instance.</returns>
        public string ToStringWith(PrintConfig config = null);

        /// <summary>
        /// Internal method to represent the content of this <c>IElement</c> as well-indented strings with all settings.
        /// </summary>
        /// <param name="config">The <c>PrintConfig</c> to direct the string representation.</param>
        /// <returns>Strings representing each <c>XTextBlock</c>, children <c>XElement</c>s, <c>Opening</c> and <c>Closing</c> tags.</returns>
        public IEnumerable<string> AsStrings([NotNull] PrintConfig config);
    }
}
