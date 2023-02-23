using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public abstract class SettingManager
    {
        protected PropertyInfo Property { get; private set; }
        protected SettingAttribute Attribute { get; private set; }

        protected string Name
        { 
            get
            {
                if (string.IsNullOrEmpty(Attribute.Name))
                {
                    return Property.Name;
                }
                else
                {
                    return Attribute.Name;
                }
            }
        }

        protected SettingManager(PropertyInfo property, SettingAttribute attribute)
        {
            Property = property;
            Attribute = attribute; 
        }

        public bool Match(string name)
        {
            return Name.ToLowerInvariant() == name.ToLowerInvariant();
        }

        public abstract void Add(SettingInstance value);

        public abstract void Apply(object obj);

        public abstract IEnumerable<SettingInstance> Get(object obj);

        public abstract void Setup();
    }

    public abstract class SettingManager<T> : SettingManager
    {
        protected SettingManager(PropertyInfo property, SettingAttribute attribute)
            : base(property, attribute)
        {
        }

        public bool TryParse(string stringValue, out T value)
        {
            try
            {
                if (Attribute.Parser == null)
                {
                    return TypeParsers.TryParse<T>(stringValue, out value);
                }
                else
                {
                    object parserObject = Attribute.Parser.GetConstructor(new Type[0]).Invoke(new object[0]);

                    if (parserObject is TypeParser<T> parser)
                    {
                        if (parser.CanParse(stringValue))
                        {
                            value = parser.Parse(stringValue);
                            return true;
                        }
                        else
                        {
                            value = default(T);
                            return false;
                        }
                    }
                    else
                    {
                        throw new ConfigParserMisconfigurationException("{0}: Parser {1} not suitable for type {2}", Name, Attribute.Parser.FullName, typeof(T).FullName);
                    }
                }
            }
            catch (ValueParseException exception)
            {
                throw new ConfigParseException(exception.Message);
            }
            catch (ConfigParseException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new ConfigParserMisconfigurationException("{0}: {1}", Name, exception.Message);
            }
        }

        public string Format(T value)
        {
            try
            {
                if (Attribute.Parser == null)
                {
                    return TypeParsers.Format(value);
                }
                else
                {
                    object parserObject = Attribute.Parser.GetConstructor(new Type[0]).Invoke(new object[0]);

                    if (parserObject is TypeParser<T> parser)
                    {
                        return parser.Format(value);
                    }
                    else
                    {
                        throw new ConfigParserMisconfigurationException("{0}: Parser {1} not suitable for type {2}", Name, Attribute.Parser.FullName, typeof(T).FullName);
                    }
                }
            }
            catch (ValueParseException exception)
            {
                throw new ConfigParseException(exception.Message);
            }
            catch (ConfigParseException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new ConfigParserMisconfigurationException("{0}: {1}", Name, exception.Message);
            }
        }
    }

    public enum SettingManagerValueType
    { 
        None,
        Default,
        Config,
    }
}
