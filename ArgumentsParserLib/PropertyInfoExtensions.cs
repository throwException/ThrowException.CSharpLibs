using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ThrowException.CSharpLibs.ArgumentsParserLib
{
    public static class PropertyInfoExtensions
    {
        public static FieldInfo GetBackingField(this PropertyInfo propertyInfo)
        {
            if (!propertyInfo.CanRead || !propertyInfo.GetGetMethod(nonPublic: true).IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            var backingField = propertyInfo.DeclaringType.GetField($"<{propertyInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField == null)
                return null;
            if (!backingField.IsDefined(typeof(CompilerGeneratedAttribute), inherit: true))
                return null;
            return backingField;
        }
    }
}
