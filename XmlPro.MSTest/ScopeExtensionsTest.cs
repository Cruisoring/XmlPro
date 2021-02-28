using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Extensions;
using XmlPro.Helpers;
using XmlPro.Interfaces;
using XmlPro.Entities;

namespace XmlPro.MSTest
{
    [TestClass]
    public class ScopeExtensionsTest
    {
        [TestMethod]
        public void TestCovers()
        {
            IScope scopeBig = new Scope(-100, 100);
            Assert.IsTrue(scopeBig.Covers(new Scope(-100, 1)));
            Assert.IsTrue(scopeBig.Covers(new Scope(0, 1)));
            Assert.IsTrue(scopeBig.Covers(new Scope(-100, 100)));
            Assert.IsTrue(scopeBig.Covers(new Scope(0, 100)));
            Assert.IsTrue(scopeBig.Covers(new Scope(99, 100)));
            Assert.IsTrue(scopeBig.Covers(new Scope(100, 100)));

            Assert.IsFalse(scopeBig.Covers(new Scope(-101, -100)));
            Assert.IsFalse(scopeBig.Covers(new Scope(100, 101)));
            Assert.IsFalse(scopeBig.Covers(new Scope(0, 101)));
            Assert.IsFalse(scopeBig.Covers(new Scope(-200, 100)));
            Assert.IsFalse(scopeBig.Covers(new Scope(-100, 101)));
            Assert.IsFalse(scopeBig.Covers(new Scope(101, 102)));
        }

        [TestMethod]
        public void TestAllIndexOfMultiple()
        {
            var array = new int[] {1, 2, 3, 1, 3, 5, 7, 1, 2, 1, 2};
            TestHelper.AssertEqual(new int[]{0, 7, 9},
                Indexer.AllIndexesOf<int>(array, new int[] { 1, 2 }));
        }

        [TestMethod]
        public void TestAllIndexesOfMultiple2()
        {
            var array = new int[] {1, 2, 1, 1, 3, 5, 7, 1, 2, 1, 2, 1, 2};
            TestHelper.AssertEqual(new int[]{0, 7}, Indexer.AllIndexesOf<int>(array, new int[] { 1, 2, 1 }));
        }


    }
}
