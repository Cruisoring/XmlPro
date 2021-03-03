using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Interfaces
{
    /// <summary>
    /// Interface to represent <c>XElement</c> that can contain other XML nodes represented as <c>IContained</c>, and also contained by <c>IContainer</c>.
    /// </summary>
    public interface IElement : IContained, IContainer
    {
        /// <summary>
        /// Name of the element that is also the name of the opening tag.
        /// </summary>
        string Name { get; }
    }
}
