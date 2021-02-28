using System;
using System.IO;
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
    public class XTagTest
    {
        // private static string xmlData = File.ReadAllText("Data\\XML.xml");

        [TestMethod]
        public void TestParseNormalTags()
        {
            string testData = File.ReadAllText("Data/Zoo.xml");
            XTag[] tags = XTag.ParseTags(testData.ToCharArray(), 0).ToArray();
            tags.ForEach(tag => Console.WriteLine(tag));
        }

        [TestMethod]
        public void TestXmlSpec()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            XTag[] tags = XTag.ParseTags(testData.ToCharArray(), 0).ToArray();
            tags.ForEach(tag => Console.WriteLine(tag));
        }

        [TestMethod]
        public void TestGetMultipleAttributes()
        {
            char[] chars = "<IMG align=\"left\" src = \"http://www.w3.org/Icons/WWW/w3c_home\" /> ".ToCharArray();
            XAttribute[] results = XAttribute.AttributesWithin(chars, 4).ToArray();
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
            XAttribute[] results = XAttribute.AttributesWithin(chars, 5).ToArray();
            Assert.IsTrue(results.Length == 2
                          && results[0].ToString() == "from=\"<tom@hcc.com>\""
                          && results[1].ToString() == "amount¥=\">500\"");
            Array.ForEach(results, r => Console.WriteLine(r.ToString()));
        }

        [TestMethod]
        public void TestAttributesWithoutValues()
        {
            char[] chars = "<node \tfrom  amount\u00a5  >".ToCharArray();
            XAttribute[] results = XAttribute.AttributesWithin(chars, 5).ToArray();
            Assert.IsTrue(results.Length == 2
                          && results[0].ToString() == "from=\"\""
                          && results[1].ToString() == "amount¥=\"\"");
            Array.ForEach(results, r => Console.WriteLine(r.ToString()));
        }
    }
}
