using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserDouble : TypeParser<double>
    {
        public override bool CanParse(string value)
        {
            return double.TryParse(value, out double dummy);
        }

        public override double Parse(string value)
        {
            try
            {
                return double.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
