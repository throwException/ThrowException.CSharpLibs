using System;

namespace ThrowException.CSharpLibs.DataObjectLib
{
    public interface IDataField
    {
        string FieldName { get; }
        bool Compare(Comparison comparison, object value);
    }

    public interface IDataField<T> : IDataField
    {
        T Value { get; set; }
    }
}
