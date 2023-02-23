using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserByte : TypeParser<byte>
    {
        public override string Format(byte value)
        {
            return value.ToString();
        }

        public override bool CanParse(string value)
        {
            return byte.TryParse(value, out byte dummy);
        }

        public override byte Parse(string value)
        {
            try
            {
                return byte.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
