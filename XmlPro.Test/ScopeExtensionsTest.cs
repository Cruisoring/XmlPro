using System;
using System.Linq;
using XmlPro.Extensions;
using XmlPro.Helpers;
using XmlPro.Interfaces;
using XmlPro.Models;
using Xunit;

namespace XmlPro.Test
{
    public class ScopeExtensionsTest
    {
        [Fact]
        public void TestCovers()
        {
            IScope scopeBig = new Scope(-100, 100);
            Assert.True(scopeBig.Covers(new Scope(-100, 1)));
            Assert.True(scopeBig.Covers(new Scope(0, 1)));
            Assert.True(scopeBig.Covers(new Scope(-100, 100)));
            Assert.True(scopeBig.Covers(new Scope(0, 100)));
            Assert.True(scopeBig.Covers(new Scope(99, 100)));
            Assert.True(scopeBig.Covers(new Scope(100, 100)));

            Assert.False(scopeBig.Covers(new Scope(-101, -100)));
            Assert.False(scopeBig.Covers(new Scope(100, 101)));
            Assert.False(scopeBig.Covers(new Scope(0, 101)));
            Assert.False(scopeBig.Covers(new Scope(-200, 100)));
            Assert.False(scopeBig.Covers(new Scope(-100, 101)));
            Assert.False(scopeBig.Covers(new Scope(101, 102)));
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
