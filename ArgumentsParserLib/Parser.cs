using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public enum ErrorAction
    { 
        Throw,
        ShortUsage,
        LongUsage,
    }

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

        public ErrorAction MissingVerbAction { get; set; }
        public ErrorAction ParseExceptionAction { get; set; }

        public Parser(params Type[] verbTypes)
        {
            MissingVerbAction = ErrorAction.Throw;
            ParseExceptionAction = ErrorAction.Throw;
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
            try
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
                    else if (possibleVerb == "help")
                    {
                        LongUsage();
                    }
                    else
                    {
                        MissingVerb();
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
                        MissingVerb();
                    }
                }
                return this;
            }
            catch (ArgumentsParseException exception)
            {
                HandleException(exception);
                return this;
            }
        }

        private void HandleException(ArgumentsParseException exception)
        { 
            switch (ParseExceptionAction)
            {
                case ErrorAction.Throw:
                    throw exception;
                case ErrorAction.ShortUsage:
                    ShortUsage();
                    break;
                case ErrorAction.LongUsage:
                    LongUsage();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void MissingVerb()
        {
            switch (MissingVerbAction)
            {
                case ErrorAction.Throw:
                    throw new ArgumentsParseException("No verb specified");
                case ErrorAction.ShortUsage:
                    ShortUsage();
                    break;
                case ErrorAction.LongUsage:
                    LongUsage();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        public Parser Parse(string commandLine)
        {
            try
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
                        MissingVerb();
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
                        MissingVerb();
                    }
                }
                return this;
            }
            catch (ArgumentsParseException exception)
            {
                HandleException(exception);
                return this;
            }
        }

        public Parser With<T>(Action<T> action)
        {
            if (_result is T options)
            {
                action(options);
            }
            return this;
        }

        public Parser ShortUsage()
        {
            var assembly = Assembly.GetEntryAssembly();
            var executableName = 
                assembly == null ? "exe" : 
                Path.GetFileName(Assembly.GetEntryAssembly().GetName().CodeBase);
            var prefix = "usage: " + executableName + " ";
            var addPrefix = true;
            var indent = string.Join("", prefix.Select(c => " "));

            foreach (var manager in _managers.Values)
            {
                var line = manager.Verb;
                foreach (var option in manager.Options)
                {
                    if (option.Positional > 0)
                    {
                        line += " <" + option.ValueShortDescription + ">";
                    }
                }

                if (addPrefix)
                {
                    Console.WriteLine(prefix + line);
                    addPrefix = false;
                }
                else
                {
                    Console.WriteLine(indent + line);
                }
            }
            return this;
        }

        public Parser LongUsage()
        {
            ShortUsage();

            int maxOptionNameWidth = _managers.Any() ? _managers.Values.Max(m => m.Options.Any() ? m.Options.Max(o => o.FullName.Length) : 0) : 1;
            var commonOptions = _managers.Any() ? _managers.Values.First().Options.ToList() : new List<OptionManager>();

            foreach (var manager in _managers.Values)
            {
                commonOptions.RemoveAll(o => !manager.Options.Any(o.IsSame));
            }

            if (commonOptions.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Common options:");
                foreach (var option in commonOptions)
                {
                    Console.WriteLine("  " + option.FullName.PadRight(maxOptionNameWidth) + "  " + option.LongDescription);
                }
            }

            foreach (var manager in _managers.Values)
            {
                var uncommonOptions = manager.Options
                    .Where(x => !commonOptions.Any(x.IsSame)).ToList();

                if (uncommonOptions.Any())
                {
                    Console.WriteLine();
                    Console.WriteLine("Options for " + manager.Verb + ":");

                    foreach (var option in uncommonOptions)
                    {
                        Console.WriteLine("  " + option.FullName.PadRight(maxOptionNameWidth) + "  " + option.LongDescription);
                    }
                }
            }

            Console.WriteLine();

            return this;
        }
    }
}
