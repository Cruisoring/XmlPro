using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string Print(PrintConfig config = null);
    }
}
