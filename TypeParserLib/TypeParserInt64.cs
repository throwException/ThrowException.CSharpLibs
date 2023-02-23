using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserInt64 : TypeParser<long>
    {
        public override string Format(long value)
        {
            return value.ToString();
        }

        public override bool CanParse(string value)
        {
            return long.TryParse(value, out long dummy);
        }

        public override long Parse(string value)
        {
            try
            {
                return long.Parse(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }

}
