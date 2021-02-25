using System;
using System.Linq;
using XmlPro.Helpers;
using Xunit;

namespace XmlPro.Test
{
    public class IndexerTest
    {
        [Fact]
        public void TestAllIndexOfSingle()
        {
            var array = new int[] {1, 2, 3, 1, 3, 5, 7, 1};
            var allIndexes = Indexer.AllIndexesOf<int>(array, 1, 1);
            Assert.Equal(allIndexes, new int[]{3, 7});
        }

        [Fact]
        public void TestAllIndexOfMultiple()
        {
            var array = new int[] {1, 2, 3, 1, 3, 5, 7, 1, 2, 1, 2};
            Assert.Equal(new int[]{0, 7, 9},
                Indexer.AllIndexesOf<int>(array, new int[] { 1, 2 }));
        }

        [Fact]
        public void TestAllIndexesOfMultiple2()
        {
            var array = new int[] {1, 2, 1, 1, 3, 5, 7, 1, 2, 1, 2, 1, 2};
            Assert.Equal(new int[]{0, 7}, Indexer.AllIndexesOf<int>(array, new int[] { 1, 2, 1 }));
        }


    }
}
