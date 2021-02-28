using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPro.Interfaces;

namespace XmlPro.Models
{
    public record XText: StringScope, IContained
    {
        public IContainer Parent { get; set; }

        public XText(char[] context, int begin, int end, IContainer parent=null) : base(context, begin, end)
        {
            Parent = parent;
        }

        public string ToString(int indentLevel, bool? includeChildren = null)
        {
            string indent = new string(ToStringIndentChar, indentLevel * ToStringIndentMultiplier);
            return $"{indent}{Text}";
        }
    }
}
