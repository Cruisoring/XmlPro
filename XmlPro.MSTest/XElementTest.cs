using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Configs;
using XmlPro.Extensions;
using XmlPro.Interfaces;
using XmlPro.Entities;

namespace XmlPro.MSTest
{
    [TestClass]
    public class XElementTest
    {
        static List<IContained> bookNodes = XElement.Conclude(
            File.ReadAllText("Data/books.xml").ToCharArray(), 
            XDocument.DefaultHtmlParseConfig).ToList();

        static void printWithConfig(PrintConfig config)
        {
            bookNodes.ForEach(node => Console.WriteLine(node.ToStringWith(config)));
        }

        static void printSelectiveLevels(PrintConfig template=null, params int[] levels)
        {
            var config = (template ?? new PrintConfig()) with
            {
                MaxLevelToShow = levels.Max(),
                ShowLevel = level => levels.Contains(level),
            };
            printWithConfig(config);
        }

        [TestMethod]
        public void TestPrintAllDetails()
        {
            printWithConfig(XDocument.ShowAllPrintConfig);
        }

        [TestMethod]
        public void TestPrintDefault()
        {
            printWithConfig(XElement.DefaultElementConfig);

            Console.WriteLine($"\n*********Print with 1 extra indent:");
            printWithConfig(XElement.DefaultElementConfig with {PrintAsLevel = 1});
        }

        [TestMethod]
        public void TestPrintWithShowLevel()
        {
            // printSelectiveLevels(XElement.DefaultElementConfig, 
            //     0, 2);
            printSelectiveLevels(XElement.DefaultElementConfig, 
                1, 3, 4);
        }

        [TestMethod]
        public void TestParseComplex()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            List<IContained> nodes = XElement.Conclude(testData.ToCharArray()).ToList();
            var nodeTexts = nodes.Select(node => ((Object) node).ToString()).Where(t => !String.IsNullOrEmpty(t));
            nodeTexts.ForEach(Console.WriteLine);
        }

        [TestMethod]
        public void TestParseComplexHtml()
        {
            string testData = File.ReadAllText("Data/XML.html");
            IEnumerable<IContained> nodes = XElement.Conclude(testData.ToCharArray());
            var nodeTexts = nodes.Select(node => ((Object) node).ToString()).Where(t => !String.IsNullOrEmpty(t));
            nodeTexts.ForEach(Console.WriteLine);
        }

        [TestMethod]
        public void TestGetElementWithIndexer()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Conclude(testData.ToCharArray());
            IElement root = nodes.Last() as IElement;
            Assert.AreEqual("header", root[4].Name);
            Assert.AreEqual("back", root[-1].Name);
        }

        [TestMethod]
        public void TestGetAttributeWithIndexer()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Conclude(testData.ToCharArray());
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
            IEnumerable<IContained> nodes = XElement.Conclude(testData.ToCharArray());
            IElement root = nodes.Last() as IElement;
            IElement header = root[element => element.Name == "header", true].First();
            IElement doctype = header[element => element.Name == "w3c-doctype"].First();
            Assert.AreEqual("W3C Recommendation", doctype.GetText());
        }

        [TestMethod]
        public void TestIndexerWithPredicateRecursively()
        {
            PrintConfig config = new PrintConfig() {MaxLevelToShow = 1, ShowDeclarative = false};
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Conclude(testData.ToCharArray());
            IElement root = nodes.Last() as IElement;
            IList<IElement> levelOneNodes = root[node => node.Level == 1, true].ToList();
            // levelOneNodes.ForEach(node => Console.WriteLine(node.Print(config)));

            IList<IElement> heads = root[node => node.Name == "head", true].ToList();
            var config2 = config with {MaxLevelToShow = 100, ShowElements = false};
            // heads.ForEach(node => Console.WriteLine(node.Print(config2)));

            IList<IElement> cdatas = root[node => node.GetText(null).Contains("CDATA"), true].ToList();
            cdatas.ForEach(node => Console.WriteLine(node.ToStringWith(config2)));
        }
        
    }
}
