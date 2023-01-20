using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class OptionManagerScalar<T> : OptionManager<T>
    {
        private OptionManagerValueType _type = OptionManagerValueType.None;
        private T _value = default(T);

        protected T Value { get { return _value; } }

        public OptionManagerScalar(PropertyInfo property, OptionAttribute attribute)
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
                    _type = OptionManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is string stringValue)
                {
                    if (!TryParse(stringValue, out _value))
                    {
                        throw new ArgumentsParserMisconfigurationException("{0}: Cannot parse default value", DisplayName);
                    }
                    _type = OptionManagerValueType.Default;
                }
                else
                {
                    throw new ArgumentsParserMisconfigurationException("{0}: Default value cannot be assigned", DisplayName);
                }
            }
        }

        public override void Add(OptionInstance option, bool config)
        {
            if (option.Values.Count() != 1)
                throw new ArgumentsParseException("Cannot set multiple value for {0}", DisplayName);
            var stringValue = option.Values.Single();

            switch (_type)
            {
                case OptionManagerValueType.None:
                case OptionManagerValueType.Default:
                    SetValue(stringValue);
                    _type = config ? OptionManagerValueType.Config : OptionManagerValueType.CommandLine;
                    break;
                case OptionManagerValueType.Config:
                    if (!config)
                    {
                        SetValue(stringValue);
                        _type = OptionManagerValueType.CommandLine;
                    }
                    else
                    {
                        throw new ArgumentsParseException("Duplicate option {0}", DisplayName);
                    }
                    break;
                case OptionManagerValueType.CommandLine:
                    if (!config)
                    {
                        throw new ArgumentsParseException("Duplicate option {0}", DisplayName);
                    }
                    /* ingore value from config */
                    break;
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
                    throw new ArgumentsParseException("Cannot parse value '{0}' for option {1}", stringValue, DisplayName);
                }
            }
            catch (ArgumentsParseException exception)
            {
                throw new ArgumentsParseException("{0} for option {1}", exception.Message, DisplayName);
            }
            catch (Exception exception)
            {
                throw new ArgumentsParseException("{0} for option {1}", exception.Message, DisplayName);
            }
        }

        public override void Apply(object obj)
        {
            if (_type == OptionManagerValueType.None)
            {
                if (Attribute.Required)
                {
                    throw new ArgumentsParseException("Required option {0} missing", DisplayName);
                }
            }
            else
            {
                Property.GetBackingField().SetValue(obj, _value);
            }
        }
    }
}
