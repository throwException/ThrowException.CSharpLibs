using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserUint16 : TypeParser<ushort>
    {
        public override bool CanParse(string value)
        {
            return ushort.TryParse(value, out ushort dummy);
        }

        public override ushort Parse(string value)
        {
            try
            {
                return ushort.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
