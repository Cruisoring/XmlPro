using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Helpers;

namespace XmlPro.MSTest
{
    [TestClass]
    public class IndexerTest
    {
        [TestMethod]
        public void TestAllIndexOfSingle()
        {
            var array = new int[] { 1, 2, 3, 1, 3, 5, 7, 1 };
            var allIndexes = Indexer.AllIndexesOf<int>(array, 1, 1);
            TestHelper.AssertEqual(allIndexes, new int[] { 3, 7 });
        }

        [TestMethod]
        public void TestAllIndexOfMultiple()
        {
            var array = new int[] { 1, 2, 3, 1, 3, 5, 7, 1, 2, 1, 2 };
            TestHelper.AssertEqual(new int[] { 0, 7, 9 },
                Indexer.AllIndexesOf<int>(array, new int[] { 1, 2 }));
        }

        [TestMethod]
        public void TestAllIndexesOfMultiple2()
        {
            var array = new int[] { 1, 2, 1, 1, 3, 5, 7, 1, 2, 1, 2, 1, 2 };
            TestHelper.AssertEqual(new int[] { 0, 7 }, Indexer.AllIndexesOf<int>(array, new int[] { 1, 2, 1 }));
        }
    }
}
