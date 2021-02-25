using System;
using System.Collections.Generic;
using System.Text;

namespace XmlPro.Interfaces
{
    public interface IScope: IComparable<IScope>
    {
        int Begin { get; }
        int End { get; }
        int Length { get; }
    }
}
