using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmlPro.MSTest
{
    public static class TestHelper
    {
        public static void AssertEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            foreach (var x in @this)
                action(x);
        }
    }
}
