using System;
using System.Linq;
using XmlPro.Extensions;
using XmlPro.Helpers;
using XmlPro.Interfaces;
using XmlPro.Models;
using Xunit;

namespace XmlPro.Test
{
    public class ScopeTest
    {
        [Fact]
        public void TestEquality()
        {
            Assert.Equal(new Scope(1, 2), new Scope(1, 2));
            Assert.True(new Scope(1, 2) == new Scope(1, 2));
            Assert.NotEqual(new Scope(1, 2), new Scope(2, 2));
        }

        [Fact]
        public void TestCompareTo()
        {
            Assert.True(new Scope(5, 6).CompareTo(new Scope(1, 2))>0);
            Assert.True(new Scope(5, 6).CompareTo(new Scope(5, 9)) == 0);
            Assert.True(new Scope(5, 6).CompareTo(new Scope(7, 8)) < 0);
            IScope[] scopes = new IScope[] {new Scope(3, 7), 
                new StringScope(new char[]{'a', 'b', 'c'},1, 2), 
                new Scope(5, 10)};
            Array.Sort(scopes);
            Assert.Equal(new StringScope(new char[]{'a', 'b', 'c'},1, 2), scopes[0]);
        }

        [Fact]
        public void TestGetText()
        {
            var chars = "Happy Birthday".ToCharArray();
            var scope = new StringScope(chars, 3, 5);
            Assert.Equal("py", scope.RawText);
        }


    }
}
