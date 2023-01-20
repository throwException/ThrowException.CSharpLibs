using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserFloat : TypeParser<float>
    {
        public override bool CanParse(string value)
        {
            return float.TryParse(value, out float dummy);
        }

        public override float Parse(string value)
        {
            try
            {
                return float.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
