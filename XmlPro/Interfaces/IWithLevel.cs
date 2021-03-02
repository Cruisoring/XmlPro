using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Interfaces
{
    public interface IWithLevel
    {
        /// <summary>
        /// Level of the current <c>IContained</c> node relative to the root, it shall be <c>Level</c> of <c>Parent</c> plus one.
        /// Can be used as filter or by <code>ToStringWithConfig()</code>.
        /// </summary>
        int Level { get; }
    }
}
