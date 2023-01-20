using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ThrowException.CSharpLibs.BytesUtilLib
{
    public static class Arguments
    {
        public static void ArgumentInRange(this int argument, int minValue, int maxValue)
        {
            if (argument > maxValue || argument < minValue)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public static void ArgumentNotNull(this object argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException();
            }
        }

        public static void DisposeIfNotNull(this IDisposable value)
        {
            if (value != null)
            {
                value.Dispose();
            }
        }
    }
}
