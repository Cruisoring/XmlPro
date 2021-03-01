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

        public int Level { get; init; }

        public IList<IElement> Children { get; init; }

        public IList<IText> Texts { get; }

        public XElement Root { get; init; }

        public XDocument([NotNull] char[] context, [NotNull] IList<IElement> children, IList<IText> texts = null) : 
            base(context, 0, context.Length)
        {
            Level = 0;
            Children = children;
            Texts = texts;
            children.ForEach(c => c.Parent = this);

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
