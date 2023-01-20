using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class ArgumentInstance
    {
        private readonly List<string> _positionals;
        private readonly List<OptionInstance> _options;

        public IEnumerable<OptionInstance> Options { get { return _options; } }
        public IEnumerable<string> Positionals { get { return _positionals; } }

        public void RemoveVerb()
        {
            _positionals.RemoveAt(0);
        }

        public void Add(string positional)
        {
            _positionals.Add(positional);
        }

        public void Add(OptionInstance option)
        {
            _options.Add(option);
        }

        public ArgumentInstance()
        {
            _positionals = new List<string>();
            _options = new List<OptionInstance>();
        }
    }
}
