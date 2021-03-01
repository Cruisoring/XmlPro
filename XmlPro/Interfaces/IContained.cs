using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPro.Enums;

namespace XmlPro.Interfaces
{
    /// <summary>
    /// Interface representing XML nodes to be contained by <c>IContainer</c> nodes.
    /// </summary>
    public interface IContained : IScope, IText
    {
        /// <summary>
        /// The container of this node.
        /// </summary>
        public IContainer Parent { get; set; }

        /// <summary>
        /// Type of this node.
        /// </summary>
        public ElementType Type { get; }

        public string ToString(int indentLevel, bool showText = true, bool? includeChildren = null);
    }
}
