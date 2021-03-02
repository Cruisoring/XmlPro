using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Entities;

namespace XmlPro.MSTest
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void TestToStringConfig()
        {
            Console.WriteLine(new PrintConfig().ToString());
            Console.WriteLine(new PrintConfig(){ MaxNodeLevelToShow = 2}.ToString());

        }
    }
}
