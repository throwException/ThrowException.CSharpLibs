using System;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class ConfigParseException : Exception
    {
        public ConfigParseException(string message, params object[] arguments)
            : base(string.Format(message, arguments))
        {
        }
    }
}
