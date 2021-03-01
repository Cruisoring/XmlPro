using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Interfaces
{
    public interface IContainer : IScope
    {
        public IList<IElement> Children { get; }
        public IList<IText> Texts { get; }

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
        public IElement this[int childIndex] { get; }

        /// <summary>
        /// Get the text of this entity as either the single piece of text with index identified, or the joined if index is null. 
        /// </summary>
        /// <param name="index">Null for all text joined by ' ', positive number for Nth text, negative index to get last Nth text.</param>
        /// <returns>Null if no text nodes contained, or decoded text of either the concerned piece with Non-null index, or joined text as a whole.</returns>
        public string GetText(int? index = null);
    }
}
