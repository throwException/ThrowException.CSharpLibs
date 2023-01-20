using System;
using System.Collections.Generic;
using System.Linq;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class Parser
    {
        public static Parser Create<T>()
        {
            return new Parser(typeof(T));
        }

        public static Parser Create<T1, T2>()
        {
            return new Parser(typeof(T1), typeof(T2));
        }

        public static Parser Create<T1, T2, T3>()
        {
            return new Parser(typeof(T1), typeof(T2), typeof(T3));
        }

        public static Parser Create<T1, T2, T3, T4>()
        {
            return new Parser(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        public static Parser Create<T1, T2, T3, T4, T5>()
        {
            return new Parser(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }

        private readonly Dictionary<string, VerbManager> _managers;
        private readonly VerbManager _default;
        private object _result;

        public Parser(params Type[] verbTypes)
        {
            _managers = new Dictionary<string, VerbManager>();

            foreach (var verbType in verbTypes)
            {
                var managerType = typeof(VerbManager<>).MakeGenericType(verbType);
                var manager = (VerbManager)managerType.GetConstructor(new Type[0]).Invoke(new object[0]);

                if (manager.Default)
                {
                    if (_default == null)
                    {
                        _default = manager;
                    }
                    else
                    {
                        throw new ArgumentsParserMisconfigurationException("Multiple default verbs");
                    }
                }

                if (manager.Verb != null)
                {
                    if (_managers.ContainsKey(manager.Verb))
                    {
                        throw new ArgumentsParserMisconfigurationException("Duplicate verb {0}", manager.Verb);
                    }
                    else
                    {
                        _managers.Add(manager.Verb, manager);
                    }
                }
            }
        }

        public Parser Parse(string[] args)
        {
            var optionParser = new OptionParser();
            var options = optionParser.Parse(args);

            if (options.Positionals.Any())
            {
                var possibleVerb = options.Positionals.First();

                if (_managers.ContainsKey(possibleVerb))
                {
                    options.RemoveVerb();
                    _result = _managers[possibleVerb].Apply(options);
                }
                else if (_default != null)
                {
                    _result = _default.Apply(options);
                }
                else
                {
                    throw new ArgumentsParseException("No verb specified");
                }
            }
            else
            {
                if (_default != null)
                {
                    _result = _default.Apply(options);
                }
                else
                {
                    throw new ArgumentsParseException("No verb specified");
                }
            }
            return this;
        }

        public Parser Parse(string commandLine)
        {
            var preParser = new PreParser();
            var optionParser = new OptionParser();
            var options = optionParser.Parse(preParser.Parse(commandLine));

            if (options.Positionals.Any())
            {
                var possibleVerb = options.Positionals.First();

                if (_managers.ContainsKey(possibleVerb))
                {
                    options.RemoveVerb();
                   _result = _managers[possibleVerb].Apply(options);
                }
                else if (_default != null)
                {
                    _result = _default.Apply(options);
                }
                else
                {
                    throw new ArgumentsParseException("No verb specified");
                }
            }
            else
            { 
                if (_default != null)
                {
                    _result = _default.Apply(options);
                }
                else
                {
                    throw new ArgumentsParseException("No verb specified");
                }
            }
            return this;
        }

        public Parser With<T>(Action<T> action)
        {
            if (_result is T options)
            {
                action(options);
            }
            return this;
        }
    }
}
