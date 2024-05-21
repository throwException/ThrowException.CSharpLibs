using System;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public interface IConditionBuilder<T>
        where T : ICondition
    {
        T Equal(string fieldname, object value);
        T NotEqual(string fieldname, object value);
        T Greater(string fieldname, object value);
        T Smaller(string fieldname, object value);
        T GreaterOrEqual(string fieldname, object value);
        T SamllerOrEqual(string fieldname, object value);
        T And(T a, T b);
        T Or(T a, T b);
        T Not(T a);
    }

    public interface ICondition
    { 
    }
}
