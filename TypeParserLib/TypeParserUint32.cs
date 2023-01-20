using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserUint32 : TypeParser<uint>
    {
        public override bool CanParse(string value)
        {
            return uint.TryParse(value, out uint dummy);
        }

        public override uint Parse(string value)
        {
            try
            {
                return uint.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
