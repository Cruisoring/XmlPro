using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Interfaces
{
    public interface IContainer : IScope
    {
        public IList<IContained> Children { get; }

        /// <summary>
        /// Indexer to get attribute by name. If AttributeBestMatch is set to true, then attribute name can be case-insensitive.
        /// </summary>
        /// <param name="attrName">Name of the attribute, case-insensitive when AttributeBestMatch is set to true.</param>
        /// <returns>Value of the attribute, null if no found or no matched.</returns>
        public string this[string attrName] { get; }

        /// <summary>
        /// Indexer to get child by index with negative number supported: returns last child when index is -1.
        /// </summary>
        /// <param name="childIndex">Positive number for normal access, negative to get element reversely.</param>
        /// <returns>The child at the position specified by childIndex.</returns>
        public IContained this[int childIndex] { get; }
    }
}
