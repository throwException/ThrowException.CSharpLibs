using System;
using System.Globalization;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserDateTime : TypeParser<DateTime>
    {
        public override string Format(DateTime value)
        {
            return value.ToString("s");
        }

        public override bool CanParse(string value)
        {
            return
                DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date1) ||
                DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date2) ||
                DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date3);
        }

        public override DateTime Parse(string value)
        {
            if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date1))
            {
                return date1.ToUniversalTime();
            }
            else if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date2))
            {
                return date2.ToUniversalTime();
            }
            else if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime date3))
            {
                return date3.ToUniversalTime();
            }
            else
            {
                throw new ValueParseException("Cannot parse datetime value '{0}'", value);
            }
        }
    }
}
