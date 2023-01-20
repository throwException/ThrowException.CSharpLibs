using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserBytes : TypeParser<byte[]>
    {
        public override bool CanParse(string value)
        {
            return Regex.IsMatch(value, @"^([a-fA-F0-9]{2})+$");
        }

        public override byte[] Parse(string value)
        {
            if (value.Length % 2 != 0)
                throw new ValueParseException("Uneven length of hex string");

            var buffer = new byte[value.Length / 2];

            for (int index = 0; index < value.Length / 2; index++)
            {
                try
                {
                    buffer[index] = byte.Parse(value.Substring(index * 2, 2), NumberStyles.HexNumber);
                }
                catch (FormatException exception)
                {
                    throw new ValueParseException(exception.Message);
                }
            }

            return buffer;
        }
    }
}
