using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class SettingManagerList<T> : SettingManager<T>
    {
        private SettingManagerValueType _type = SettingManagerValueType.None;
        private readonly List<T> _values = new List<T>();

        public SettingManagerList(PropertyInfo property, SettingAttribute attribute)
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
                    _type = SettingManagerValueType.Default;
                }
                else if (Attribute.DefaultValue.GetType() == typeof(T[]))
                {
                    _values.AddRange((T[])Attribute.DefaultValue);
                    _type = SettingManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is string stringValue)
                {
                    try
                    {
                        if (!TryParse(stringValue, out T value))
                        {
                            throw new ConfigParserMisconfigurationException("{0}: Cannot parse value '{1}'", Name, stringValue);
                        }
                        _values.Add(value);
                    }
                    catch (Exception exception)
                    {
                        throw new ConfigParserMisconfigurationException("{0}: {1}", Name, exception.Message);
                    }
                    _type = SettingManagerValueType.Default;
                }
                else if (Attribute.DefaultValue is string[] stringValues)
                {
                    foreach (var oneStringValue in stringValues)
                    {
                        try
                        {
                            if (!TryParse(oneStringValue, out T value))
                            {
                                throw new ConfigParserMisconfigurationException("{0}: Cannot parse value '{1}'", Name, oneStringValue);
                            }
                            _values.Add(value);
                        }
                        catch (Exception exception)
                        {
                            throw new ConfigParserMisconfigurationException("{0}: {1}", Name, exception.Message);
                        }
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
                case SettingManagerValueType.Config:
                    AddStringValue(value.Value);
                    _type = SettingManagerValueType.Config;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void AddStringValue(string stringValue)
        {
            try
            {
                if (!TryParse(stringValue, out T value))
                {
                    throw new ConfigParseException("Cannot parse value '{0}' for config setting {1}", stringValue, Name);
                }
                _values.Add(value);
            }
            catch (ConfigParseException)
            {
                throw;
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

        public override IEnumerable<SettingInstance> Get(object obj)
        {
            foreach (var value in Property.GetBackingField().GetValue(obj) as IEnumerable<T>)
            {
                yield return new SettingInstance(Name, Format(value));
            }
        }
    }
}
