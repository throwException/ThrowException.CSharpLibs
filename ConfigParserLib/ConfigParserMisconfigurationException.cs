using System;

namespace ThrowException.CSharpLibs.ConfigParserLib
{
    public class ConfigParserMisconfigurationException : Exception
    {
        public ConfigParserMisconfigurationException(string message, params object[] arguments)
            : base(string.Format(message, arguments))
        {
        }
    }
}
