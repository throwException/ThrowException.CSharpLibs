using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserMisconfigurationException : Exception
    {
        public TypeParserMisconfigurationException(string message, params object[] arguments)
            : base(string.Format(message, arguments))
        {
        }
    }
}
