using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlPro.Enums;

namespace XmlPro.Interfaces
{
    public interface IContained : IScope
    {
        public IContainer Parent { get; set; }

        public ElementType Type { get; }

        public string ToString(int indentLevel, bool? includeChildren = null);
    }
}
