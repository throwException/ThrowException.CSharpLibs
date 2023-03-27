using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class XmlConfig<T>
        where T : IConfig, new()
    {
        public T ParseFile(string filename)
        {
            return Parse(XDocument.Load(filename));
        }

        public T Parse(string xmlData)
        {
            return Parse(Encoding.UTF8.GetBytes(xmlData));
        }

        public T Parse(byte[] xmlData)
        {
            using (var stream = new MemoryStream(xmlData))
            {
                return Parse(XDocument.Load(stream));
            }
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

        public void FormatSave(T value, string filename)
        {
            var document = FormatDocument(value);
            using (var stream = File.OpenWrite(filename))
            {
                document.Save(stream);
            }
        }

        public string FormatString(T value)
        {
            return FormatDocument(value).ToString();
        }

        public byte[] FormatData(T value)
        {
            return Encoding.UTF8.GetBytes(FormatString(value));
        }

        public XDocument FormatDocument(T value)
        {
            return new XDocument(FormatElement(value));
        }

        private XElement FormatElement(T value)
        {
            var manager = new ConfigManager<T>();
            return FormatSetting(manager.Get(value, "config"));
        }

        private XElement FormatSetting(SettingInstance value)
        {
            var element = (value.Value != null) ? new XElement(value.Name, value.Value) : new XElement(value.Name);
            foreach (var child in value.Children)
            {
                element.Add(FormatSetting(child));
            }
            return element;
        }
    }
}
