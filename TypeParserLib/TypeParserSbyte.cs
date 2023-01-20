using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserSbyte : TypeParser<sbyte>
    {
        public override bool CanParse(string value)
        {
            return sbyte.TryParse(value, out sbyte dummy);
        }

        public override sbyte Parse(string value)
        {
            try
            {
                return sbyte.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
