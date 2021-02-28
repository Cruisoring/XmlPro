using System;
using System.Collections.Generic;
using System.Text;

namespace XmlPro.Models
{
    public record XElement: StringScope
    {
        public XElement(char[] context, int begin, int end) : base(context, begin, end)
        {
        }
    }
}
