using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Extensions;
using XmlPro.Interfaces;
using XmlPro.Entities;

namespace XmlPro.MSTest
{
    [TestClass]
    public class XElementTest
    {
        [TestMethod]
        public void TestParseSimple()
        {
            string testData = File.ReadAllText("Data/books.xml");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            var nodeTexts = nodes.Select(node => node.ToString()).Where(t => !String.IsNullOrEmpty(t));
            nodeTexts.ForEach(Console.WriteLine);
        }

        [TestMethod]
        public void TestParseComplex()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            var nodeTexts = nodes.Select(node => node.ToString()).Where(t => !String.IsNullOrEmpty(t));
            nodeTexts.ForEach(Console.WriteLine);
        }

        [TestMethod]
        public void TestParseComplexHtml()
        {
            string testData = File.ReadAllText("Data/XML.html");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            var nodeTexts = nodes.Select(node => node.ToString()).Where(t => !String.IsNullOrEmpty(t));
            nodeTexts.ForEach(Console.WriteLine);
        }
    }
}
