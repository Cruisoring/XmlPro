using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using XmlPro.Interfaces;

namespace XmlPro.Helpers
{
    public static class Indexer
    {
        public static int IndexOf<T>([NotNull] T[] context, [NotNull] T[] part, int startIndex = 0) where T: IComparable<T>
        {
            int partLength = part.Length;
            int contextLength = context.Length;

            if (partLength > contextLength)
                return -1;

            for (int i = startIndex; i < contextLength - partLength + 1; i++)
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

        public static int IndexOfAny<T>([NotNull] T[] context, [NotNull] T[] options, int startIndex = 0) where T : IComparable<T>
        {
            if (options.Length == 0)
            {
                return -1;
            }

            int contextLength = context.Length;
            for (int i = startIndex; i < contextLength; i++)
            {
                if (options.Contains(context[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static IEnumerable<int> AllIndexesOf<T>([NotNull] T[] context, T key, int startIndex = 0)
            where T: IComparable<T>
        {
            for (int i = Math.Max(0, startIndex); i < context.Length; i++)
            {
                if (key.CompareTo(context[i]) == 0)
                    yield return i;
            }
        }
        
        public static IEnumerable<int> AllIndexesOf<T>([NotNull] T[] context, [NotNull] T[] part, int startIndex = 0)
            where T : IComparable<T>
        {
            int partLength = part.Length;
            int start = startIndex < 0 ? 0 : startIndex;
            int last = context.Length - partLength;
            do
            {
                int index = IndexOf(context, part, start);
                if (index < 0)
                {
                    yield break;
                }

                start = index + partLength;
                yield return index;
            } while (start <= last);
        }

    }
}
