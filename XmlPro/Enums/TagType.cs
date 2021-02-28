using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlPro.Enums
{
    public enum TagType
    {
        Unknown,
        Opening,
        Closing,
        Sound,

        Remark,
        CDATA,
        DocType,
        Declaration
    }
}
