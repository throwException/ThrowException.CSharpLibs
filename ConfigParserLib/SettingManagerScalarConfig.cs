using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class SettingManagerScalarConfig<T> : SettingManagerConfig<T>
        where T : IConfig, new()
    {
        private SettingManagerValueType _type = SettingManagerValueType.None;
        private T _value = default(T);

        protected T Value { get { return _value; } }

        public SettingManagerScalarConfig(PropertyInfo property, SettingAttribute attribute)
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
                    _value = Create(value);
                    _type = SettingManagerValueType.Config;
                    break;
                case SettingManagerValueType.Config:
                    throw new ConfigParseException("Duplicate config value for {0}", Name);
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
            }
            else
            {
                Property.GetBackingField().SetValue(obj, _value);
            }
        }

        public override IEnumerable<SettingInstance> Get(object obj)
        {
            T value = (T)Property.GetBackingField().GetValue(obj);
            yield return Get(value);
        }
    }

}
