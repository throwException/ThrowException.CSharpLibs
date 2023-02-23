using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public static class TypeParsers
    {
        private static readonly Dictionary<Type, List<TypeParser>> _parsers;

        static TypeParsers()
        {
            _parsers = new Dictionary<Type, List<TypeParser>>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            { 
                var baseType = type.BaseType;
                if (!type.IsGenericType &&
                    baseType.IsConstructedGenericType &&
                    (baseType.GenericTypeArguments.Length == 1) &&
                    (baseType == typeof(TypeParser<>).MakeGenericType(baseType.GenericTypeArguments.Single())))
                {
                    var parsedType = baseType.GenericTypeArguments.Single();
                    var parser = (TypeParser)type.GetConstructor(new Type[0]).Invoke(new object[0]);

                    if (!_parsers.ContainsKey(parsedType))
                    {
                        _parsers.Add(parsedType, new List<TypeParser>());
                    }
                    _parsers[parsedType].Add(parser);
                }
            }
        }

        public static void ResetConfiguration()
        { 
            foreach (var parser in _parsers.Values.SelectMany(p => p))
            {
                parser.Reset();
            }
        }

        public static TypeParser Get(Type parserType)
        {
            var baseType = parserType.BaseType;

            if (baseType.IsConstructedGenericType &&
                (baseType.GenericTypeArguments.Length == 1) &&
                (baseType == typeof(TypeParser<>).MakeGenericType(baseType.GenericTypeArguments.Single())))
            {
                var parsedType = baseType.GenericTypeArguments.Single();
                if (!_parsers.ContainsKey(parsedType))
                {
                    _parsers.Add(parsedType, new List<TypeParser>());
                }

                var parser = _parsers[parsedType].SingleOrDefault(p => p.GetType() == parserType);
                if (parser == null)
                {
                    parser = (TypeParser)parserType.GetConstructor(new Type[0]).Invoke(new object[0]);
                    _parsers[parsedType].Add(parser);
                }
                return parser;
            }
            else
            {
                throw new ArgumentException("Not a parser type.");
            }
        }

        public static IEnumerable<TypeParser<T>> Get<T>()
        {
            var parsedType = typeof(T);

            if (parsedType.IsEnum)
            {
                if (_parsers.ContainsKey(parsedType))
                {
                    foreach (var parser in _parsers[parsedType]
                        .Where(p => p.Enabled)
                        .OrderByDescending(p => p.Precedence))
                    {
                        yield return (TypeParser<T>)parser;
                    }
                }
                else
                {
                    var parserType = typeof(TypeParserEnum<>).MakeGenericType(parsedType);
                    var parser = (TypeParser<T>)parserType.GetConstructor(new Type[0]).Invoke(new object[0]);
                    _parsers.Add(parsedType, new List<TypeParser>());
                    _parsers[parsedType].Add(parser);
                    yield return parser;
                }
            }
            else
            {
                if (!_parsers.ContainsKey(parsedType))
                    throw new TypeParserMisconfigurationException("No parser for type {0}", parsedType.FullName);

                foreach (var parser in _parsers[parsedType]
                    .Where(p => p.Enabled)
                    .OrderByDescending(p => p.Precedence))
                {
                    yield return (TypeParser<T>)parser;
                }
            }
        }

        public static bool TryParse<T>(string s, out T v)
        {
            var parsers = Get<T>();

            foreach (var parser in parsers
                .Where(p => p.Enabled)
                .OrderByDescending(p => p.Precedence))
            {
                if (parser.CanParse(s))
                {
                    v = parser.Parse(s);
                    return true;
                }
            }


            v = default(T);
            return false;
        }

        public static string Format<T>(T v)
        {
            var parsers = Get<T>();

            foreach (var parser in parsers
                .Where(p => p.Enabled)
                .OrderByDescending(p => p.Precedence))
            {
                return parser.Format(v);
            }

            return null;
        }
    }
}
