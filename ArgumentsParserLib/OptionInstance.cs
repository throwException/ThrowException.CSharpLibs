using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class OptionInstance
    {
        private readonly List<string> _values;

        public char? ShortName { get; private set; }
        public string LongName { get; private set; }
        public IEnumerable<string> Values { get { return _values; } }

        public OptionInstance(string longName, params string[] values)
            : this(longName)
        {
            _values.AddRange(values);
        }

        public OptionInstance(char shortName, params string[] values)
            : this(shortName)
        {
            _values.AddRange(values);
        }

        public OptionInstance(char shortName)
        {
            _values = new List<string>();
            ShortName = shortName;
            LongName = null;
        }

        public OptionInstance(string longName)
        {
            _values = new List<string>();
            ShortName = null;
            LongName = longName;
        }

        public void Add(string value)
        {
            _values.Add(value); 
        }
    }
}
