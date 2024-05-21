using System;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public interface IDataField<T>
    {
        T Value { get; set; }
    }
}
