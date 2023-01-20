using System;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public enum OptionType
    {
        Standard,
        Flag,
        CountedFlag,
        IniConfigFile,
        XmlConfigFile,
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OptionAttribute : Attribute
    {
        public char? ShortName { get; private set; }
        public string LongName { get; private set; }
        public object DefaultValue { get; set; }
        public OptionType Type { get; set; }
        public Type Parser { get; set; }
        public uint Positional { get; set; }
        public bool Required { get; set; }

        public OptionAttribute(string longName)
        {
            ShortName = null;
            LongName = longName;
            Setup();
        }

        public OptionAttribute(char shortName, string longName)
        {
            ShortName = shortName;
            LongName = longName;
            Setup();
        }

        private void Setup()
        {
            Required = true;
            Positional = 0;
            DefaultValue = null;
            Type = OptionType.Standard;
        }
    }
}
