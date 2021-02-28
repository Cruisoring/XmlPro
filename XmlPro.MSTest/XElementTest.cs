using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Extensions;
using XmlPro.Interfaces;
using XmlPro.Models;

namespace XmlPro.MSTest
{
    [TestClass]
    public class XElementTest
    {
        [TestMethod]
        public void TestParseSimple()
        {
            string testData = File.ReadAllText("Data/Zoo.xml");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            nodes.ForEach(node => Console.WriteLine(node.ToString(0, true)));
        }

        [TestMethod]
        public void TestParseComplex()
        {
            string testData = File.ReadAllText("Data/XML.xml");
            IEnumerable<IContained> nodes = XElement.Parse(testData.ToCharArray(), 0);
            nodes.ForEach(node => Console.WriteLine(node.ToString(0, true)));
        }
    }
}
