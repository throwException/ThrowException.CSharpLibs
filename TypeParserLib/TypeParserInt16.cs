using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserInt16 : TypeParser<short>
    {
        public override bool CanParse(string value)
        {
            return short.TryParse(value, out short dummy);
        }

        public override short Parse(string value)
        {
            try
            {
                return short.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
