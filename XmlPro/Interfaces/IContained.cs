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
    public interface IContained : IScope, IText
    {
        /// <summary>
        /// The container of this node, could be updated by the closing tag of the <c>Parent</c> node.
        /// </summary>
        public IContainer Parent { get; set; }

        /// <summary>
        /// Level of the current <c>IContained</c> node relative to the root, it shall be <c>Level</c> of <c>Parent</c> plus one.
        /// Can be used as filter or by <code>ToStringWithConfig()</code>.
        /// </summary>
        public int Level { get; init; }

        /// <summary>
        /// Type of this node.
        /// </summary>
        public ElementType Type { get; }

        public string Print(PrintConfig config = null);
    }
}
