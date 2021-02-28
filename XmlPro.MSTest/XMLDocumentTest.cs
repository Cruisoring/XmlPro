using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmlPro.Test
{
    [TestClass]
    public class XMLDocumentTest
    {
        [TestMethod]
        public void TestXmlDocumentParsing()
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load("Data\\XML.xml");
            XmlNode rootNode = doc.DocumentElement;
            DisplayNodes(rootNode);
            // Console.ReadLine();
        }

        private static void DisplayNodes(XmlNode node)
        {
            //Print the node type, node name and node value of the node
            if (node.NodeType == XmlNodeType.Text)
            {
                Console.WriteLine("Type = [" + node.NodeType + "] Value = " + node.Value);
            }
            else
            {
                Console.WriteLine("Type = [" + node.NodeType + "] Name = " + node.Name);
            }

            //Print attributes of the node
            if (node.Attributes != null)
            {
                XmlAttributeCollection attrs = node.Attributes;
                foreach (XmlAttribute attr in attrs)
                {
                    Console.WriteLine("Attribute Name = " + attr.Name + "; Attribute Value = " + attr.Value);
                }
            }

            //Print individual children of the node, gets only direct children of the node
            XmlNodeList children = node.ChildNodes;
            foreach (XmlNode child in children)
            {
                DisplayNodes(child);
            }
        }
    }
}
