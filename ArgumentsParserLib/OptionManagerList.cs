using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class OptionManagerList<T> : OptionManager<T>
    {
        private OptionManagerValueType _type = OptionManagerValueType.None;
        private readonly List<T> _values = new List<T>();

        public OptionManagerList(PropertyInfo property, OptionAttribute attribute)
            : base(property, attribute)
        {
        }

        public override void Setup()
        {
            if (Attribute.DefaultValue != null)
            {
                if (Attribute.DefaultValue.GetType() == typeof(T))
                {
                    _values.Add((T)Attribute.DefaultValue);
                    _type = OptionManagerValueType.Default;
                }
                else if (Attribute.DefaultValue.GetType() == typeof(T[]))
                {
                    _values.AddRange((T[])Attribute.DefaultValue);
                    _type = OptionManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is string stringValue)
                {
                    try
                    {
                        if (!TryParse(stringValue, out T value))
                        {
                            throw new ArgumentsParserMisconfigurationException("{0}: Cannot parse value '{1}'", DisplayName, stringValue);
                        }
                        _values.Add(value);
                    }
                    catch (Exception exception)
                    {
                        throw new ArgumentsParserMisconfigurationException("{0}: {1}", DisplayName, exception.Message);
                    }
                    _type = OptionManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is string[] stringValues)
                {
                    foreach (var oneStringValue in stringValues)
                    {
                        try
                        {
                            if (!TryParse(oneStringValue, out T value))
                            {
                                throw new ArgumentsParserMisconfigurationException("{0}: Cannot parse value '{1}'", DisplayName, oneStringValue);
                            }
                            _values.Add(value);
                        }
                        catch (Exception exception)
                        {
                            throw new ArgumentsParserMisconfigurationException("{0}: {1}", DisplayName, exception.Message);
                        }
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
                    AddStringValues(option);
                    _type = config ? OptionManagerValueType.Config : OptionManagerValueType.CommandLine;
                    break;
                case OptionManagerValueType.Config:
                    if (!config)
                    {
                        _values.Clear();
                        AddStringValues(option);
                        _type = OptionManagerValueType.CommandLine;
                    }
                    else
                    {
                        AddStringValues(option);
                    }
                    break;
                case OptionManagerValueType.CommandLine:
                    if (!config)
                    {
                        AddStringValues(option);
                    }
                    /* ignore values from config */
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void AddStringValues(OptionInstance option)
        {
            foreach (var stringValue in option.Values)
            {
                try
                {
                    if (!TryParse(stringValue, out T value))
                    {
                        throw new ArgumentsParseException("Cannot parse value '{0}' for option {1}", stringValue, DisplayName);
                    }
                    _values.Add(value);
                }
                catch (ArgumentsParseException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw new ArgumentsParseException("{0} for option {1}", exception.Message, DisplayName);
                }
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
                Property.GetBackingField().SetValue(obj, _values);
            }
        }
    }
}
