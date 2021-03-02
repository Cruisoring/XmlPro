using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Extensions;
using XmlPro.Helpers;
using XmlPro.Interfaces;
using XmlPro.Entities;
using XmlPro.MSTest;

namespace XmlPro.Test
{
    [TestClass]
    public class XAttributeTest
    {

        [TestMethod]
        public void TestEmptyTags()
        {
            XAttribute[] results = XAttribute.ParseAttributes("<td>".ToCharArray(), 3).ToArray();
            Assert.IsTrue(results.Length == 0);

            results = XAttribute.ParseAttributes("<td />".ToCharArray(), 3).ToArray();
            Assert.IsTrue(results.Length == 0);
        }

        [TestMethod]
        public void TestTagsWithSingleAttribute()
        {
            XAttribute[] results = XAttribute.ParseAttributes("<td x=\"abc\">".ToCharArray(), 3).ToArray();
            Assert.IsTrue(results.Length == 1 && results[0].ToString() == "x=\"abc\"");
            Console.WriteLine(results[0].RawText);
            
            results = XAttribute.ParseAttributes("<td  x=\"abc\" />".ToCharArray(), 3).ToArray();
            Assert.IsTrue(results.Length == 1 && results[0].ToString() == "x=\"abc\"");
            Console.WriteLine(results[0].RawText);
        }

        [TestMethod]
        public void TestGetMultipleAttributes()
        {
            char[] chars = "<IMG align=\"left\" src = \"http://www.w3.org/Icons/WWW/w3c_home\" /> ".ToCharArray();
            XAttribute[] results = XAttribute.ParseAttributes(chars, 4).ToArray();
            Assert.IsTrue(results.Length == 2
                          && results[0].ToString() == "align=\"left\""
                          && results[1].ToString() == "src=\"http://www.w3.org/Icons/WWW/w3c_home\""
            );
            Array.ForEach(results, r => Console.WriteLine(r.ToString()));
        }

        [TestMethod]
        public void TestAttributesWithSpecialChars()
        {
            char[] chars = "<node from=\"&lt;tom@hcc.com&gt;\" amount\u00a5=\"&gt;500\">".ToCharArray();
            XAttribute[] results = XAttribute.ParseAttributes(chars, 5).ToArray();
            Assert.IsTrue(results.Length == 2
                          && results[0].ToString() == "from=\"<tom@hcc.com>\""
                          && results[1].ToString() == "amount¥=\">500\"");
            Array.ForEach(results, r => Console.WriteLine(r.ToString()));
        }

        [TestMethod]
        public void TestAttributesWithoutValues()
        {
            char[] chars = "<node \tfrom  amount\u00a5  >".ToCharArray();
            XAttribute[] results = XAttribute.ParseAttributes(chars, 5).ToArray();
            Assert.IsTrue(results.Length == 2
                          && results[0].ToString() == "from"
                          && results[1].ToString() == "amount¥");
            Array.ForEach(results, r => Console.WriteLine(r.ToString()));
        }
    }
}
