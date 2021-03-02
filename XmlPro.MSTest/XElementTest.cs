using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            List<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0).ToList();
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

        [TestMethod]
        public void TestGetElementWithIndexer()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            IElement root = nodes.Last() as IElement;
            Assert.AreEqual("header", root[4].Name);
            Assert.AreEqual("back", root[-1].Name);
        }

        [TestMethod]
        public void TestGetAttributeWithIndexer()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            IElement xml = nodes.First() as IElement;
            Assert.AreEqual("1.0", xml["version"]);
            Assert.AreEqual("UTF-8", xml["Encoding"]);

            IElement root = nodes.Last() as IElement;
            Assert.AreEqual("sec-intro", root[5][0]["id"]);
        }

        [TestMethod]
        public void TestGetNodesWithPredicate()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            IElement root = nodes.Last() as IElement;
            IElement header = root[node => node is IElement element && element.Name == "header"].First();
            IElement doctype = header[node => node is IElement element && element.Name == "w3c-doctype"].First();
            Assert.AreEqual("W3C Recommendation", doctype.GetText());
        }

        [TestMethod]
        public void TestIndexerWithPredicateRecursively()
        {
            PrintConfig config = new PrintConfig() {MaxNodeLevelToShow = 1, ShowDeclarative = false};
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            IElement root = nodes.Last() as IElement;
            IList<IElement> levelOneNodes = root[node => node.Level == 1, true].ToList();
            // levelOneNodes.ForEach(node => Console.WriteLine(node.Print(config)));

            IList<IElement> heads = root[node => node.Name == "head", true].ToList();
            var config2 = config with {MaxNodeLevelToShow = 100, ShowElements = false};
            // heads.ForEach(node => Console.WriteLine(node.Print(config2)));

            IList<IElement> cdatas = root[node => node.GetText(null).Contains("CDATA"), true].ToList();
            cdatas.ForEach(node => Console.WriteLine(node.Print(config2)));
        }
        
    }
}
