using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public abstract class OptionManager
    {
        protected PropertyInfo Property { get; private set; }
        protected OptionAttribute Attribute { get; private set; }

        protected string DisplayName
        { 
            get
            {
                if (!string.IsNullOrEmpty(Attribute.LongName) && Attribute.ShortName.HasValue)
                {
                    return "--" + Attribute.LongName + " (-" + Attribute.ShortName.Value + ")";
                }
                else if (!string.IsNullOrEmpty(Attribute.LongName))
                {
                    return "--" + Attribute.LongName;
                }
                else if (Attribute.ShortName.HasValue)
                {
                    return "-" + Attribute.ShortName.Value;
                }
                else
                {
                    return "?";
                }
            }
        }

        public OptionManager(PropertyInfo property, OptionAttribute attribute)
        {
            Property = property;
            Attribute = attribute; 
        }

        public bool Match(char shortName)
        {
            return Attribute.ShortName.HasValue &&
                Attribute.ShortName.Value == shortName;
        }

        public bool Match(string longName)
        {
            return Attribute.LongName == longName;
        }

        public abstract void Add(OptionInstance option, bool config);

        public abstract void Apply(object obj);

        public abstract void Setup();

        public uint Positional { get { return Attribute.Positional; } }
    }

    public enum OptionManagerValueType
    { 
        None,
        Default,
        Config,
        CommandLine,
    }

    public abstract class OptionManager<T> : OptionManager
    {
        protected OptionManager(PropertyInfo property, OptionAttribute attribute)
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
                        throw new ArgumentsParserMisconfigurationException("{0}: Parser {1} not suitable for type {2}", DisplayName, Attribute.Parser.FullName, typeof(T).FullName);
                    }
                }
            }
            catch (ValueParseException exception)
            {
                throw new ArgumentsParseException(exception.Message);
            }
            catch (ArgumentsParseException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new ArgumentsParserMisconfigurationException("{0}: {1}", DisplayName, exception.Message);
            }
        }
    }
}
