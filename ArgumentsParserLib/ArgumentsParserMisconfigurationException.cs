using System;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public class ArgumentsParserMisconfigurationException : Exception
    {
        public ArgumentsParserMisconfigurationException(string message, params object[] arguments)
            : base(string.Format(message, arguments))
        {
        }
    }
}
