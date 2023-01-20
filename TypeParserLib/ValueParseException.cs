using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class ValueParseException : Exception
    {
        public ValueParseException(string message, params object[] arguments)
            : base(string.Format(message, arguments))
        {
        }
    }
}
