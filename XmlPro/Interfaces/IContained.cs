using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Interfaces
{
    public interface IContained
    {
        public IContainer Parent { get; set; }

        public string ToString(int indentLevel, bool? includeChildren = null);
    }
}
