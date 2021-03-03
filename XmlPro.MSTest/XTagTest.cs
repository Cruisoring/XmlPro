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
            XTag[] tags = XTag.GetEnumerator(testData.ToCharArray()).ToArray();
            tags.ForEach(tag => Console.WriteLine(tag));
        }

        [TestMethod]
        public void TestXmlSpec()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            XTag[] tags = XTag.GetEnumerator(testData.ToCharArray()).ToArray();
            tags.ForEach(tag => Console.WriteLine(tag));
        }

        [TestMethod]
        public void TestDescriptiveTags()
        {
            string sampleTag = "<?xml-stylesheet type=\"text/xsl\" href=\"REC-xml.xsl\"?>";
            XTag tag = XTag.GetEnumerator(sampleTag.ToCharArray()).First();
            Console.WriteLine(tag.ToString());
            
            sampleTag = @"<!--
Notes on preparation of the Fifth Edition:
	
- Worked from http://www.w3.org/XML/xml-V10-4e-errata -->";
            tag = XTag.GetEnumerator(sampleTag.ToCharArray()).First();
            Console.WriteLine(tag.Print(XDocument.ShowAllPrintConfig));
        }

        [TestMethod]
        public void TestPrint()
        {
            string sampleTag = "<sample&amp; key&lt;='Tom&apos; book' noValue authors&gt;=\"Eric &amp; Frank\" />";
            XTag tag = XTag.GetEnumerator(sampleTag.ToCharArray()).First();
            Console.WriteLine(tag.ToString());
            Assert.AreEqual("<sample& sample&=\"Eric & Frank\" sample&=\"Tom' book\" sample& />", 
                tag.ToString());
            Console.WriteLine(tag.Print(XDocument.ShowAllPrintConfig));
            Assert.AreEqual("<sample&amp; key&lt;=\"Tom&#39; book\" noValue authors&gt;=\"Eric &amp; Frank\" />",
                tag.Print(XDocument.ShowAllPrintConfig));
        }
    }
}
