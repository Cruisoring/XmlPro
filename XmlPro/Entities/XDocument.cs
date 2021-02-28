using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlPro.Enums;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record XDocument : StringScope, IContainer
    {
        public static readonly ElementType[] RootElementTypes = new[] {ElementType.Compound, ElementType.Simple};


        public IEnumerable<IContained> Children { get; init; }

        public XElement Root { get; init; }

        public XDocument(char[] context, IEnumerable<IContained> children) : 
            base(context, children.First().Begin, children.Last().End)
        {
            Children = children;

            Root = children.Single(c => RootElementTypes.Contains(c.Type)) as XElement;
            
        }

    }
}
