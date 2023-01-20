using System;
using System.Linq;
using System.Reflection;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class OptionManagerFlag : OptionManager
    {
        private OptionManagerValueType _type = OptionManagerValueType.Default;
        private bool _value = false;

        public OptionManagerFlag(PropertyInfo property, OptionAttribute attribute)
            : base(property, attribute)
        {
        }

        public override void Setup()
        {
            if (Attribute.DefaultValue != null)
            {
                if (Attribute.DefaultValue is bool value)
                {
                    _value = value;
                    _type = OptionManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is string stringValue)
                {
                    try
                    {
                        if (!TypeParsers.TryParse<bool>(stringValue, out _value))
                        {
                            throw new ArgumentsParserMisconfigurationException("{0}: Cannot parse value '{1}'", DisplayName, stringValue);
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new ArgumentsParserMisconfigurationException("{0}: {1}", DisplayName, exception.Message);
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
            switch (_type)
            {
                case OptionManagerValueType.None:
                case OptionManagerValueType.Default:
                    if (!option.Values.Any())
                    {
                        _value = true;
                        _type = config ? OptionManagerValueType.Config : OptionManagerValueType.CommandLine;
                    }
                    else
                    {
                        throw new ArgumentsParseException("Cannot set value for flag {0}", DisplayName);
                    }
                    break;
                case OptionManagerValueType.Config:
                    if (!config)
                    {
                        if (!option.Values.Any())
                        {
                            _value = true;
                            _type = OptionManagerValueType.CommandLine;
                        }
                        else
                        {
                            throw new ArgumentsParseException("Cannot set value for flag {0}", DisplayName);
                        }
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
                    /* ignore values from config file */
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public override void Apply(object obj)
        {
            Property.GetBackingField().SetValue(obj, _value);
        }
    }
}
