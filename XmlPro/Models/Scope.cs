using System;
using System.Collections.Generic;
using System.Text;
using XmlPro.Interfaces;

namespace XmlPro.Models
{
    public record Scope : IScope, IComparable<Scope>
    {

        public int Begin { get; }
        public int End { get; }
        public int Length => End - Begin;


        public static implicit operator Scope((int begin, int end) range)
        {
            return new Scope(range.begin, range.end);
        }

        public Scope(int begin, int end)
        {
            if (begin > end)
            {
                throw new IndexOutOfRangeException($"{begin} shall not be bigger than {end}.");
            }

            Begin = begin;
            End = end;
        }


        public int CompareTo(IScope other)
        {
            return this.Begin.CompareTo(other.Begin);
        }

        public int CompareTo(Scope other)
        {
            return this.Begin.CompareTo(other.Begin);
        }

        public override string ToString() => $"[{Begin}, {End})";
    }
}
