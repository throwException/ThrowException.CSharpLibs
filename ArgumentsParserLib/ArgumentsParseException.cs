using System;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class ArgumentsParseException : Exception
    {
        public ArgumentsParseException(string message, params object[] arguments)
            : base(string.Format(message, arguments))
        {
        }
    }
}
