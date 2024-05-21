using System;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DefaultValueAttribute : Attribute
    {
        public object Value { get; private set; }

        public DefaultValueAttribute(object value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NullableAttribute : Attribute
    {
        public bool Value { get; private set; }

        public NullableAttribute(bool value)
        {
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public ColumnNameAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableNameAttribute : Attribute
    {
        public string Name { get; private set; }

        public TableNameAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LoadingAttribute : Attribute
    {
        public LoadingBehavior Behavior { get; private set; }

        public LoadingAttribute(LoadingBehavior behavior)
        {
            Behavior = behavior;
        }
    }
}
