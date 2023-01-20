using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserTimeSpan : TypeParser<TimeSpan>
    {
        private static bool TryParseTimeSpan(string value, out TimeSpan time)
        {
            var result = Regex.Match(value, @"^((?:[+\-])?[0-9]+(?:\.[0-9]+)?)([a-z]+)$");
            if (result.Success)
            {
                if (double.TryParse(result.Groups[1].Value, out double number))
                { 
                    switch (result.Groups[2].Value)
                    {
                        case "d":
                            time = TimeSpan.FromDays(number);
                            return true;
                        case "h":
                            time = TimeSpan.FromHours(number);
                            return true;
                        case "m":
                            time = TimeSpan.FromMinutes(number);
                            return true;
                        case "s":
                            time = TimeSpan.FromSeconds(number);
                            return true;
                        case "ms":
                            time = TimeSpan.FromMilliseconds(number);
                            return true;
                        case "qs":
                            time = TimeSpan.FromMilliseconds(number / 1000d);
                            return true;
                        case "ns":
                            time = TimeSpan.FromMilliseconds(number / 1000000d);
                            return true;
                        default:
                            time = TimeSpan.Zero;
                            return false;
                    }
                }
                else
                {
                    time = TimeSpan.Zero;
                    return false;
                }
            }
            else
            {
                time = TimeSpan.Zero;
                return false;
            }
        }

        private IEnumerable<string> FormatStrings
        { 
            get
            {
                yield return @"hh\:mm\:ss";
                yield return @"d\ hh\:mm\:ss";
                yield return @"d\.hh\:mm\:ss";
                yield return @"hh\:mm\:ss\.f";
                yield return @"d\ hh\:mm\:ss\.f";
                yield return @"d\.hh\:mm\:ss\.f";
                yield return @"hh\:mm\:ss\.ff";
                yield return @"d\ hh\:mm\:ss\.ff";
                yield return @"d\.hh\:mm\:ss\.ff";
                yield return @"hh\:mm\:ss\.fff";
                yield return @"d\ hh\:mm\:ss\.fff";
                yield return @"d\.hh\:mm\:ss\.fff";
                yield return @"hh\:mm\:ss\.fff";
                yield return @"d\ hh\:mm\:ss\.ffff";
                yield return @"d\.hh\:mm\:ss\.ffff";
                yield return @"hh\:mm\:ss\.fffff";
                yield return @"d\ hh\:mm\:ss\.ffffff";
                yield return @"d\.hh\:mm\:ss\.ffffff";
                yield return @"hh\:mm\:ss\.ffffff";
                yield return @"d\ hh\:mm\:ss\.fffffff";
                yield return @"d\.hh\:mm\:ss\.fffffff";
            }
        }

        public override bool CanParse(string value)
        {
            foreach (var formatString in FormatStrings)
            {
                if (TimeSpan.TryParseExact(value, formatString, CultureInfo.InvariantCulture, out TimeSpan time1))
                {
                    return true;
                }
            }
            return TryParseTimeSpan(value, out TimeSpan time2);
        }

        public override TimeSpan Parse(string value)
        {
            foreach (var formatString in FormatStrings)
            {
                if (TimeSpan.TryParseExact(value, formatString, CultureInfo.InvariantCulture, out TimeSpan time1))
                {
                    return time1;
                }
            }
            if (TryParseTimeSpan(value, out TimeSpan time2))
            {
                return time2;
            }
            else
            {
                throw new ValueParseException("Cannot parse datetime value '{0}'", value);
            }
        }
    }
}
