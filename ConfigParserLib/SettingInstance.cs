using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class SettingInstance
    {
        private readonly List<SettingInstance> _children;

        public string Name { get; private set; }
        public string Value { get; private set; }

        public IEnumerable<SettingInstance> Children { get { return _children; } }

        public SettingInstance(string name, string value)
        {
            _children = new List<SettingInstance>();
            Name = name;
            Value = value;
        }

        public void Add(SettingInstance child)
        {
            _children.Add(child);
        }
    }
}
