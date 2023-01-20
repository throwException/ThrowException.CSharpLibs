using System;
using System.IO;
using System.Xml.Linq;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class XmlConfigParser<T>
        where T : IConfig, new()
    {
        public T ParseFile(string filename)
        {
            return Parse(XDocument.Load(filename));
        }

        public T Parse(string xmlData)
        {
            return Parse(XDocument.Parse(xmlData));
        }

        public T Parse(XDocument document)
        {
            var root = Parse(document.Root);
            var manager = new ConfigManager<T>();
            return manager.Apply(root);
        }

        private SettingInstance Parse(XElement element)
        {
            var setting = new SettingInstance(element.Name.LocalName, element.Value);

            foreach (var subElement in element.Elements())
            {
                setting.Add(Parse(subElement));
            }

            return setting;
        }
    }
}
