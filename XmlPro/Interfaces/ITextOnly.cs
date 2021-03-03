using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Interfaces
{
    public interface ITextOnly: IContained
    {
        /// <summary>
        /// User-friendly decoded text.
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Print the content as either encoded or raw outerText with given indent.
        /// </summary>
        /// <param name="indent">Indent to hint level of this <c>ITextOnly</c> instance.</param>
        /// <param name="encodeContent"><c>TRUE</c> to show the raw content, otherwise the user-friendly content.</param>
        /// <returns>String representing the content shifted with indent.</returns>
        string AsIndented(string indent = "", bool encodeContent = false);
    }
}
