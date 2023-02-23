using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserUint64 : TypeParser<ulong>
    {
        public override string Format(ulong value)
        {
            return value.ToString();
        }

        public override bool CanParse(string value)
        {
            return ulong.TryParse(value, out ulong dummy);
        }

        public override ulong Parse(string value)
        {
            try
            {
                return ulong.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
