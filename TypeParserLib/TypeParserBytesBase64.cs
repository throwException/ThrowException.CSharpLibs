using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserBytesBase64 : TypeParser<byte[]>
    {
        public override string Format(byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        public TypeParserBytesBase64()
        {
            Enabled = false;
        }

        public override void Reset()
        {
            Enabled = false;
            Precedence = 0;
        }

        public override bool CanParse(string value)
        {
            return Regex.IsMatch(value, @"^(?:[A-Za-z0-9+\/]{4})*(?:[A-Za-z0-9+\/]{4}|[A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}={2})$");
        }

        public override byte[] Parse(string value)
        {
            try
            {
                return Convert.FromBase64String(value);
            }
            catch (FormatException exception)
            {
                throw new ValueParseException(exception.Message);
            }
        }
    }
}
