using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserBool : TypeParser<bool>
    {
        public override bool CanParse(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "0":
                case "f":
                case "false":
                case "n":
                case "no":
                case "1":
                case "t":
                case "true":
                case "y":
                case "yes":
                    return true;
                default:
                    return false;
            }
        }

        public override bool Parse(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "0":
                case "f":
                case "false":
                case "n":
                case "no":
                    return false;
                case "1":
                case "t":
                case "true":
                case "y":
                case "yes":
                    return true;
                default:
                    throw new ValueParseException("Cannot parse bool value '{0}'", value);
            }
        }
    }
}
