using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class OptionManagerConfigFile : OptionManagerScalar<string>
    {
        public OptionManagerConfigFile(PropertyInfo property, OptionAttribute attribute) 
            : base(property, attribute)
        {
            Attribute.Parser = typeof(TypeParserFilename);
        }

        private ArgumentInstance ReadIniConfigFile()
        {
            var result = new ArgumentInstance();
            if (!string.IsNullOrEmpty(Value))
            {
                foreach (var line in File.ReadAllLines(Value))
                {
                    var trimmed = line.Trim();
                    if (trimmed.Contains("="))
                    {
                        var index = trimmed.IndexOf("=", StringComparison.Ordinal);
                        var name = trimmed.Substring(0, index).Trim();
                        var value = trimmed.Substring(index + 1).Trim();
                        if (name.Length > 0)
                        {
                            result.Add(new OptionInstance(name, value));
                        }
                    }
                    else
                    {
                        if (trimmed.Length > 0)
                        {
                            result.Add(new OptionInstance(trimmed));
                        }
                    }
                }
            }
            return result;
        }

        private ArgumentInstance ReadXmlConfigFile()
        {
            var result = new ArgumentInstance();
            if (!string.IsNullOrEmpty(Value))
            {
                var document = XDocument.Load(Value);
                foreach (var element in document.Root.Elements())
                {
                    result.Add(new OptionInstance(element.Name.LocalName, element.Value));
                }
            }
            return result;
        }

        public ArgumentInstance ReadConfigFile()
        {
            switch (Attribute.Type)
            {
                case OptionType.IniConfigFile:
                    return ReadIniConfigFile();
                case OptionType.XmlConfigFile:
                    return ReadXmlConfigFile();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
