using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public abstract class VerbManager
    {
        public string Verb { get; protected set; }
        public bool Default { get; protected set; }
        public abstract object Apply(ArgumentInstance arguments);
        public abstract IEnumerable<OptionManager> Options { get; }
    }

    public class VerbManager<T> : VerbManager
        where T : new()
    {
        private readonly List<OptionManager> _options;

        public override IEnumerable<OptionManager> Options => _options;

        private OptionManager CreateSettingManagerScalar(Type baseType, PropertyInfo property, OptionAttribute attribute)
        {
            var finalType = typeof(OptionManagerScalar<>).MakeGenericType(baseType);
            var option = (OptionManager)finalType
                .GetConstructor(new Type[] { typeof(PropertyInfo), typeof(OptionAttribute) })
                .Invoke(new object[] { property, attribute });
            option.Setup();
            return option;
        }

        private OptionManager CreateSettingManagerList(Type listType, PropertyInfo property, OptionAttribute attribute)
        {
            var finalType = typeof(OptionManagerList<>).MakeGenericType(listType);
            var option = (OptionManager)finalType
                .GetConstructor(new Type[] { typeof(PropertyInfo), typeof(OptionAttribute) })
                .Invoke(new object[] { property, attribute });
            option.Setup();
            return option;
        }

        public VerbManager()
        {
            _options = new List<OptionManager>();

            var verbType = typeof(T);

            var verbAttribute = (VerbAttribute)verbType
                .GetCustomAttributes(typeof(VerbAttribute), true)
                .SingleOrDefault();

            if (verbAttribute == null)
            {
                throw new ArgumentsParserMisconfigurationException("Verb attribute missing for type {0}", verbType.FullName);
            }

            Verb = verbAttribute.Verb;
            Default = verbAttribute.Default;

            foreach (var property in verbType.GetProperties())
            {
                var optionAttribute = (OptionAttribute)property
                    .GetCustomAttributes(typeof(OptionAttribute), true)
                    .SingleOrDefault();

                if (optionAttribute != null)
                {
                    var baseType = property.PropertyType;
                    var displayName = "?";

                    if (!string.IsNullOrEmpty(optionAttribute.LongName))
                    {
                        displayName = "--" + optionAttribute.LongName;

                        if (optionAttribute.ShortName.HasValue)
                        {
                            displayName += " (" + "-" + optionAttribute.ShortName.Value + ")";
                        }
                    }
                    else if (optionAttribute.ShortName.HasValue)
                    {
                        displayName = "-" + optionAttribute.ShortName.Value;
                    }

                    switch (optionAttribute.Type)
                    {
                        case OptionType.Standard:
                            if (baseType.IsConstructedGenericType &&
                                (baseType.GenericTypeArguments.Length == 1))
                            {
                                var genericTypeArgument = baseType.GenericTypeArguments.Single();
                                if (baseType == typeof(List<>).MakeGenericType(genericTypeArgument))
                                {
                                    _options.Add(CreateSettingManagerList(genericTypeArgument, property, optionAttribute));
                                }
                                else if (baseType == typeof(IEnumerable<>).MakeGenericType(genericTypeArgument))
                                {
                                    _options.Add(CreateSettingManagerList(genericTypeArgument, property, optionAttribute));
                                }
                                else
                                {
                                    _options.Add(CreateSettingManagerScalar(baseType, property, optionAttribute));
                                }
                            }
                            else
                            {
                                _options.Add(CreateSettingManagerScalar(baseType, property, optionAttribute));
                            }
                            break;
                        case OptionType.Flag:
                            if (baseType == typeof(bool))
                            {
                                var option = new OptionManagerFlag(property, optionAttribute);
                                option.Setup();
                                _options.Add(option);
                            }
                            else
                            {
                                throw new ArgumentsParserMisconfigurationException("{0}: Flag type is not bool", displayName);
                            }
                            break;
                        case OptionType.CountedFlag:
                            if (baseType == typeof(uint))
                            {
                                var option = new OptionManagerCountedFlag(property, optionAttribute);
                                option.Setup();
                                _options.Add(option);
                            }
                            else
                            {
                                throw new ArgumentsParserMisconfigurationException("{0}: Counted flag type is not uint", displayName);
                            }
                            break;
                        case OptionType.IniConfigFile:
                        case OptionType.XmlConfigFile:
                            if (baseType == typeof(string))
                            {
                                optionAttribute.Parser = typeof(TypeParserFilename);
                                var option = new OptionManagerConfigFile(property, optionAttribute);
                                option.Setup();
                                _options.Add(option);
                            }
                            else
                            {
                                throw new ArgumentsParserMisconfigurationException("{0}: Config file type is not string", displayName);
                            }
                            break;
                    }
                }
            }
        }

        private OptionManager GetSettingManager(OptionInstance option)
        {
            OptionManager result = null;

            if (!string.IsNullOrEmpty(option.LongName))
            {
                result = _options.SingleOrDefault(o => o.Match(option.LongName));
            }
            else if (option.ShortName.HasValue)
            {
                result = _options.SingleOrDefault(o => o.Match(option.ShortName.Value));
            }
            else
            {
                throw new ArgumentException("Option without a shortname or longname");
            }

            if (result == null)
            {
                var displayName = "?";
                if (!string.IsNullOrEmpty(option.LongName))
                {
                    displayName = "--" + option.LongName;
                }
                else if (option.ShortName.HasValue)
                {
                    displayName = "-" + option.ShortName.Value.ToString();
                }

                throw new ArgumentsParseException("Unknown option {0}", displayName);
            }

            return result;
        }

        public override object Apply(ArgumentInstance arguments)
        {
            T result = new T();

            foreach (var option in arguments.Options)
            {
                var manager = GetSettingManager(option);
                manager.Add(option, false);
            }

            foreach (var configManager in _options.OfType<OptionManagerConfigFile>())
            {
                var config = configManager.ReadConfigFile();
                foreach (var option in config.Options)
                {
                    var manager = GetSettingManager(option);
                    manager.Add(option, true);
                }
            }

            {
                OptionManager manager = null;
                for (int i = 0; i < arguments.Positionals.Count(); i++)
                {
                    var managers = _options.Where(m => m.Positional == (i + 1));
                    var positionalValue = arguments.Positionals.ElementAt(i);

                    switch (managers.Count())
                    {
                        case 0:
                            if (manager == null)
                            {
                                throw new ArgumentsParseException("Superflous positional argument");
                            }
                            else
                            {
                                manager.Add(new OptionInstance(string.Empty, positionalValue), false);
                            }
                            break;
                        case 1:
                            manager = managers.Single();
                            manager.Add(new OptionInstance(string.Empty, positionalValue), false);
                            break;
                        default:
                            throw new ArgumentsParserMisconfigurationException("Multiple positional argument {0}", i);
                    }
                }
            }

            foreach (var manager in _options)
            {
                manager.Apply(result);
            }

            return result;
        }
    }
}
