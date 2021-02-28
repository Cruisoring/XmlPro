using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPro.Enums;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record XText: StringScope, IContained
    {
        public IContainer Parent { get; set; }

        public ElementType Type => ElementType.Text;

        public XText(char[] context, int begin, int end) : base(context, begin, end)
        {
        }

        public string ToString(int indentLevel, bool? includeChildren = null)
        {
            string indent = new string(ToStringIndentChar, indentLevel * ToStringIndentMultiplier);
            return $"{indent}{Text}";
        }
    }
}
