using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserString : TypeParser<string>
    {
        public override bool CanParse(string value)
        {
            return true;
        }

        public override string Parse(string value)
        {
            return value;
        }
    }
}
