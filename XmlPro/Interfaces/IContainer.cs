using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Interfaces
{
    /// <summary>
    /// Interface to represent XML nodes that can contain other nodes of <c>IElement</c> or <c>IText</c> types.
    /// </summary>
    public interface IContainer : IScope, IWithLevel
    {
        /// <summary>
        /// Contained <c>IElement</c> nodes as a list with original order for easier filtering and accessing.
        /// </summary>
        public IList<IElement> Elements { get; }

        /// <summary>
        /// Contained <c>IText</c> nodes as a list with original order for easier filtering and accessing.
        /// </summary>
        public IList<ITextOnly> Texts { get; }

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
        /// Indexer to get elements qualified by filter.
        /// </summary>
        /// <param name="filter">The filter to get the concerned <c>IElement</c>s in their original orders.</param>
        /// <param name="recursively">Apply filter to the children of the <c>IElement</c> to get matched nodes recursively.</param>
        /// <returns>All children matching the given condition.</returns>
        public IEnumerable<IElement> this[Predicate<IElement> filter, bool recursively = true] { get; }

        /// <summary>
        /// Get the text of this entity as either the single piece of text with index identified, or the joined if index is null. 
        /// </summary>
        /// <param name="index">Null for all text joined by ' ', positive number for Nth text, negative index to get last Nth text.</param>
        /// <param name="connector">Applied only when <c>index</c> is null to join all <c>XText</c> strings together.</param>
        /// <returns>Null if no text nodes contained, or decoded text of either the concerned piece with Non-null index, or joined text as a whole.</returns>
        public string GetText(int? index = null, char connector = '\n');

        /// <summary>
        /// Internal method to represent the content of this <c>IElement</c> as well-indented strings with all settings.
        /// </summary>
        /// <param name="maxLevel">The maximum level of nodes to be shown.</param>
        /// <param name="showLevel">Predicate to select only specific levels.</param>
        /// <param name="attrOrderByName"><c>True</c> to sort the attributes of the opening tag, otherwise keep their original orders.</param>
        /// <param name="showDeclarative"><c>True</c> to show the Declarative nodes (Remark, CDATA, DocType and Declaration).</param>
        /// <param name="showTexts">True to show the XText objects at their presenting positions, False to hide texts.</param>
        /// <param name="showElements">True to include contained XElements as indented strings.</param>
        /// <param name="trimText">True to trim each <c>ITextOnly</c> instance before showing.</param>
        /// <param name="encodeContent">True to encode the text contents, otherwise show decoded texts that could make output as invalid XML.</param>
        /// <param name="encodeAttrName">True to encode the attribute names, otherwise show decoded texts that could make output as invalid XML.</param>
        /// <param name="encodeAttrValue">True to encode the attribute values, otherwise show decoded texts that could make output as invalid XML.</param>
        /// <param name="currentIndent">The leading string to be appended before the lines.</param>
        /// <param name="moreIndent">Extra indent to be appended.</param>
        /// <param name="childConnector">String to connect multiple <c>IElement</c> or <c>ITextOnly</c> instances.</param>
        /// <returns>Strings representing each <c>XTextBlock</c>, children <c>XElement</c>s, <c>Opening</c> and <c>Closing</c> tags.</returns>
        public IEnumerable<string> AsIndented(
            int maxLevel,
            Predicate<int> showLevel,
            bool attrOrderByName,
            bool showDeclarative, bool showTexts, bool showElements,
            bool trimText,
            bool encodeContent, bool encodeAttrName, bool encodeAttrValue,
            string currentIndent, string moreIndent, string childConnector);
    }
}
