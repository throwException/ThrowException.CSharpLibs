using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ThrowException.CSharpLibs.ConfigParserLib;
using NUnit.Framework;

namespace ThrowException.CSharpLibs.ConfigParserTest
{
    public class Config : IConfig
    {
        [Setting]
        public int Number { get; private set; }

        [Setting(DefaultValue = 42.23d)]
        public double Value { get; private set; }

        [Setting]
        public IEnumerable<string> Strings { get; private set; }

        [Setting]
        public SubConfig Sub { get; private set; }

        [Setting]
        public IEnumerable<SubConfig> Subs { get; private set; }
    }

    public class SubConfig : IConfig
    {
        [Setting]
        public int Number { get; private set; }
    }

    [TestFixture()]
    public class ParserTest
    {
        [Test()]
        public static void XmlConfigTest()
        {
            var document = new XDocument(
                new XElement("config",
                    new XElement("number", 23),
                    new XElement("strings", "t1"),
                    new XElement("strings", "t2"),
                    new XElement("strings", "t3"),
                    new XElement("sub", 
                        new XElement("number", 17)),
                    new XElement("subs",
                        new XElement("number", 1337)),
                    new XElement("subs",
                        new XElement("number", 1338))));
            var text = document.ToString();

            var parser = new XmlConfig<Config>();
            var config = parser.Parse(text);

            Assert.AreEqual(config.Number, 23);
            Assert.AreEqual(config.Value, 42.23d);
            Assert.AreEqual(config.Strings.Count(), 3);
            Assert.AreEqual(config.Strings.ElementAt(0), "t1");
            Assert.AreEqual(config.Strings.ElementAt(1), "t2");
            Assert.AreEqual(config.Strings.ElementAt(2), "t3");
            Assert.AreEqual(config.Sub.Number, 17);
            Assert.AreEqual(config.Subs.Count(), 2);
            Assert.AreEqual(config.Subs.ElementAt(0).Number, 1337);
            Assert.AreEqual(config.Subs.ElementAt(1).Number, 1338);

            var data = parser.FormatData(config);
            var config2 = parser.Parse(data);

            Assert.AreEqual(config2.Number, 23);
            Assert.AreEqual(config2.Value, 42.23d);
            Assert.AreEqual(config2.Strings.Count(), 3);
            Assert.AreEqual(config2.Strings.ElementAt(0), "t1");
            Assert.AreEqual(config2.Strings.ElementAt(1), "t2");
            Assert.AreEqual(config2.Strings.ElementAt(2), "t3");
            Assert.AreEqual(config2.Sub.Number, 17);
            Assert.AreEqual(config2.Subs.Count(), 2);
            Assert.AreEqual(config2.Subs.ElementAt(0).Number, 1337);
            Assert.AreEqual(config2.Subs.ElementAt(1).Number, 1338);
        }
    }
}
