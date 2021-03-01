using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XmlPro.Interfaces
{
    /// <summary>
    /// Interface to define a closed-open range for further operations.
    /// </summary>
    public interface IScope: IComparable<IScope>
    {
        /// <summary>
        /// Inclusive start of the IScope.
        /// </summary>
        int Begin { get; }

        /// <summary>
        /// Exclusive end of the IScope, shall be no less than <c>Begin</c>.
        /// </summary>
        int End { get; }

        /// <summary>
        /// Length of the IScope, shall equal to <c>End</c>-<c>Begin</c>.
        /// </summary>
        int Length { get; }
    }
}
