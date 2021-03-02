using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using XmlPro.Helpers;
using XmlPro.Interfaces;
using XmlPro.Entities;

namespace XmlPro.Extensions
{
    public static class ScopeExtensions
    {
        public delegate IScope ToScope(int begin, int end);

        public static ToScope DefaultToScope = (int begin, int end) => new Scope(begin, end);


        public static bool Covers(this IScope scope, IScope other)
        {
            return scope.Begin <= other.Begin && scope.End >= other.End;
        }

        public static IScope IntersectWith(this IScope scope, IScope other, ToScope creator = null)
        {
            creator ??= DefaultToScope;
            int begin = Math.Min(scope.Begin, other.Begin);
            int end = Math.Max(scope.End, other.End);

            return creator(begin, end);
        }

        public static IScope OfScope<T>(this T[] context, int begin = 0, int? end = null)
        {
            int _begin = begin < 0 ? context.Length + begin : begin;
            int _end = end == null ? context.Length : (end < 0 ? context.Length + end.Value : end.Value);
            return new Scope(_begin, _end);
        }

        public static IElement AsElement([NotNull] string context, int level)
        {
            char[] chars = context.ToCharArray();
            IEnumerable<IContained> nodes = XElement.Parse(chars, 0, chars.Length, null);
            XElement first = nodes.First() as XElement;
            return first with { Level = level };
        }


        public static int IndexOf<T>(this IScope scope, T[] context, T[] part) where T: IComparable<T>
        {
            if (scope.Begin < 0 || scope.End >= context.Length)
                return -1;

            int partLength = part.Length;
            int last = scope.End - partLength + 1;
            for (int i = scope.Begin; i < last; i++)
            {
                int j = 0;
                for (int k = i; j < partLength; j++, k++)
                {
                    if (part[j].CompareTo(context[k]) != 0)
                        break;
                }

                if (j == partLength)
                    return i;
            }

            return -1;
        }

        public static IEnumerable<int> AllIndexesOf<T>(this IScope scope, [NotNull] T[] context, [NotNull] T[] part)
            where T: IComparable<T>
        {
            int partLength = part.Length;
            int start = scope.Begin < 0 ? 0 : scope.Begin;
            int last = Math.Min(context.Length, scope.Length) - partLength;
            do
            {
                int index = Indexer.IndexOf<T>(context, part, start);
                if (index < 0)
                {
                    yield break;
                }
                start = index + partLength;
                yield return index;
            } while (start <= last);
        }


        public static string TextFrom(this IScope scope, [NotNull] char[] source)
        {
            if (scope.Begin < 0 || scope.End > source.Length)
            {
                throw new IndexOutOfRangeException();
            }

            return new string(source, scope.Begin, scope.Length);
        }

        public static string GetText(IList<IWithText> texts, int? index=null, char connector = '\n')
        {
            if (texts == null)
            {
                return "";
            }
            else if (index == null)
            {
                return string.Join(connector, texts.Select(t => t.Text));
            }
            else if (texts.Count == 0 || index >= texts.Count || index < -texts.Count)
            {
                throw new IndexOutOfRangeException($"Index {index} is out of range with texts of {texts.Count}");
            }
            else
            {
                return texts[index.Value < 0 ? texts.Count + index.Value : index.Value].Text;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var x in @this)
                action(x);
        }
    }
}
