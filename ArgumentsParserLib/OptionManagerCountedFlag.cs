using System;
using System.Linq;
using System.Reflection;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class OptionManagerCountedFlag : OptionManager
    {
        private OptionManagerValueType _type = OptionManagerValueType.Default;
        private uint _value = 0;

        public OptionManagerCountedFlag(PropertyInfo property, OptionAttribute attribute)
            : base(property, attribute)
        {
        }

        public override void Setup()
        {
            if (Attribute.DefaultValue != null)
            {
                if (Attribute.DefaultValue is uint uintValue)
                {
                    _value = uintValue;
                    _type = OptionManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is int intValue)
                {
                    _value = (uint)intValue;
                    _type = OptionManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is string stringValue)
                {
                    try
                    {
                        if (!TypeParsers.TryParse<uint>(stringValue, out _value))
                        {
                            throw new ArgumentsParserMisconfigurationException("{0}: Cannot parse value '{1}'", DebugName, stringValue);
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new ArgumentsParserMisconfigurationException("{0}: {1}", DebugName, exception.Message);
                    }
                    _type = OptionManagerValueType.Default;
                }
                else
                {
                    throw new ArgumentsParserMisconfigurationException("{0}: Default value cannot be assigned", DebugName);
                }
            }
        }

        public override void Add(OptionInstance option, bool config)
        {
            if (!option.Values.Any())
            {
                switch (_type)
                {
                    case OptionManagerValueType.None:
                    case OptionManagerValueType.Default:
                        _value = 1;
                        _type = config ? OptionManagerValueType.Config : OptionManagerValueType.CommandLine;
                        break;
                    case OptionManagerValueType.Config:
                        if (!config)
                        {
                            _value = 1;
                            _type = OptionManagerValueType.CommandLine;
                        }
                        else
                        {
                            _value++;
                        }
                        break;
                    case OptionManagerValueType.CommandLine:
                        if (!config)
                        {
                            _value++;
                        }
                        /* ingore value from config */
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                throw new ArgumentsParseException("Cannot set value for flag {0}", DebugName);
            }
        }

        public override void Apply(object obj)
        {
            Property.GetBackingField().SetValue(obj, _value);
        }
    }
}
