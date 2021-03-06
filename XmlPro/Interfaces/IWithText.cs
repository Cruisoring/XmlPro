﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XmlPro.Interfaces
{
    /// <summary>
    /// Interface to represent <c>XText</c> nodes.
    /// </summary>
    public interface IWithText
    {
        /// <summary>
        /// Original content of the <c>IText</c> instance that might contain decoded characters.
        /// </summary>
        string RawOuterText { get; }

        /// <summary>
        /// Content of the <c>IText</c> instance for display purposes.
        /// </summary>
        string OuterText { get; }
    }
}
