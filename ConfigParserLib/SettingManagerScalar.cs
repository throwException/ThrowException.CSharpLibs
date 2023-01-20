using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class SettingManagerScalar<T> : SettingManager<T>
    {
        private SettingManagerValueType _type = SettingManagerValueType.None;
        private T _value = default(T);

        protected T Value { get { return _value; } }

        public SettingManagerScalar(PropertyInfo property, SettingAttribute attribute)
            : base(property, attribute)
        {
        }

        public override void Setup()
        {
            if (Attribute.DefaultValue != null)
            {
                if (Attribute.DefaultValue.GetType() == typeof(T))
                {
                    _value = (T)Attribute.DefaultValue;
                    _type = SettingManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is string stringValue)
                {
                    if (!TryParse(stringValue, out _value))
                    {
                        throw new ConfigParserMisconfigurationException("{0}: Cannot parse default value", Name);
                    }
                    _type = SettingManagerValueType.Default;
                }
                else
                {
                    throw new ConfigParserMisconfigurationException("{0}: Default value cannot be assigned", Name);
                }
            }
        }

        public override void Add(SettingInstance value)
        {
            switch (_type)
            {
                case SettingManagerValueType.None:
                case SettingManagerValueType.Default:
                    SetValue(value.Value);
                    _type = SettingManagerValueType.Config;
                    break;
                case SettingManagerValueType.Config:
                    throw new ConfigParseException("Duplicate config value for {0}", Name);
                default:
                    throw new NotSupportedException();
            }
        }

        private void SetValue(string stringValue)
        {
            try
            {
                if (!TryParse(stringValue, out _value))
                {
                    throw new ConfigParseException("Cannot parse value '{0}' for config setting {1}", stringValue, Name);
                }
            }
            catch (ConfigParseException exception)
            {
                throw new ConfigParseException("{0} for config setting {1}", exception.Message, Name);
            }
            catch (Exception exception)
            {
                throw new ConfigParseException("{0} for config setting {1}", exception.Message, Name);
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
    }
}
