using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Extensions;
using XmlPro.Helpers;
using XmlPro.Interfaces;
using XmlPro.Models;
using XmlPro.MSTest;

namespace XmlPro.Test
{
    [TestClass]
    public class ScopeTest
    {
        [TestMethod]
        public void TestEquality()
        { 
            Assert.AreEqual(new Scope(1, 2), new Scope(1, 2));
            Assert.IsTrue(new Scope(1, 2) == new Scope(1, 2));
            Assert.AreNotEqual(new Scope(1, 2), new Scope(2, 2));
        }

        [TestMethod]
        public void TestCompareTo()
        {
            Assert.IsTrue(new Scope(5, 6).CompareTo(new Scope(1, 2))>0);
            Assert.IsTrue(new Scope(5, 6).CompareTo(new Scope(5, 9)) == 0);
            Assert.IsTrue(new Scope(5, 6).CompareTo(new Scope(7, 8)) < 0);
            IScope[] scopes = new IScope[] {new Scope(3, 7), 
                new StringScope(new char[]{'a', 'b', 'c'},1, 2), 
                new Scope(5, 10)};
            Array.Sort(scopes);
            Assert.AreEqual("[1, 2): b", scopes[0].ToString());
        }

        [TestMethod]
        public void TestGetText()
        {
            var chars = "Happy Birthday".ToCharArray();
            var scope = new StringScope(chars, 3, 5);
            TestHelper.AssertEqual("py", scope.RawText);
        }


    }
}
