using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserInt32 : TypeParser<int>
    {
        public override bool CanParse(string value)
        {
            return int.TryParse(value, out int dummy);
        }

        public override int Parse(string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
