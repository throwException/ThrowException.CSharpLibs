using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserDecimal : TypeParser<decimal>
    {
        public override bool CanParse(string value)
        {
            return decimal.TryParse(value, out decimal dummy);
        }

        public override decimal Parse(string value)
        {
            try
            {
                return decimal.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
