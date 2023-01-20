using System;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SettingAttribute : Attribute
    {
        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public Type Parser { get; set; }
        public bool Required { get; set; }

        public SettingAttribute()
        {
            Name = null;
            Required = true;
            DefaultValue = null;
        }
    }
}
