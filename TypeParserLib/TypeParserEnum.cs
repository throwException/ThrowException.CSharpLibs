using System;

namespace ThrowException.CSharpLibs.TypeParserLib
{
    public class TypeParserEnum<T> : TypeParser<T>
        where T : struct, Enum
    {
        public override bool CanParse(string value)
        {
            return Enum.TryParse<T>(value, out T dummy);
        }

        public override T Parse(string value)
        {
            if (Enum.TryParse<T>(value, out T enumValue))
            {
                return enumValue;
            }
            else
            {
                throw new ArgumentException(string.Format("Cannot parse value '{0}' for enum {1}", value, typeof(T).FullName));
            }
        }
    }
}
