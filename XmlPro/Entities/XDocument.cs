using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using XmlPro.Enums;
using XmlPro.Extensions;
using XmlPro.Interfaces;

namespace XmlPro.Entities
{
    public record XDocument : StringScope, IContainer
    {
        public static readonly ElementType[] RootElementTypes = new[] {ElementType.Compound, ElementType.Simple};


        public IList<IElement> Children { get; init; }

        public IList<IText> Texts { get; }

        public XElement Root { get; init; }

        public XDocument([NotNull] char[] context, [NotNull] IList<IElement> children, IList<IText> texts = null) : 
            base(context, 0, context.Length)
        {
            Children = children;
            Texts = texts;

            Root = children.Single(c => RootElementTypes.Contains(c.Type)) as XElement;
        }

        public string this[string attrName] => Root[attrName];

        public IElement this[int childIndex] => Root[childIndex];

        public string GetText(int? index = null)
        {
            return ScopeExtensions.GetText(Texts, index);
        }
    }
}
