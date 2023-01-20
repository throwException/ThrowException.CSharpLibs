using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class SettingManagerListConfig<T> : SettingManagerConfig<T>
        where T : IConfig, new()
    {
        private SettingManagerValueType _type = SettingManagerValueType.None;
        private readonly List<T> _values = new List<T>();

        public SettingManagerListConfig(PropertyInfo property, SettingAttribute attribute)
            : base(property, attribute)
        {
        }

        public override void Setup()
        {
            if (Attribute.DefaultValue != null)
            {
                throw new ConfigParserMisconfigurationException("{0}: Default value for settings of type config not supported", Name);
            }
        }

        public override void Add(SettingInstance value)
        {
            switch (_type)
            {
                case SettingManagerValueType.None:
                case SettingManagerValueType.Default:
                case SettingManagerValueType.Config:
                    _values.Add(Create(value));
                    _type = SettingManagerValueType.Config;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Apply(object obj)
        {
            if (_type == SettingManagerValueType.None)
            {
                if (Attribute.Required)
                {
                    throw new ConfigParseException("Required config setting {0} missing", Name);
                }
                else
                {
                    Property.GetBackingField().SetValue(obj, _values);
                }
            }
            else
            {
                Property.GetBackingField().SetValue(obj, _values);
            }
        }
    }

}
