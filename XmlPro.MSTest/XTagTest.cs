using System;
using System.IO;
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

    }
}
