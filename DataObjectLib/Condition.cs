using System;
using System.Collections.Generic;
using System.Linq;

namespace ThrowException.CSharpLibs.DataObjectLib
{
    public abstract class Condition
    {
        public abstract bool Match(IDataObject obj);

        public static CompareCondition Equal(string fieldName, object value)
        {
            return new CompareCondition(fieldName, Comparison.Equal, value);
        }

        public static CompareCondition NotEqual(string fieldName, object value)
        {
            return new CompareCondition(fieldName, Comparison.NotEqual, value);
        }

        public static CompareCondition Greater(string fieldName, object value)
        {
            return new CompareCondition(fieldName, Comparison.Greater, value);
        }

        public static CompareCondition GreaterOrEqual(string fieldName, object value)
        {
            return new CompareCondition(fieldName, Comparison.GreaterOrEqual, value);
        }

        public static CompareCondition Smaller(string fieldName, object value)
        {
            return new CompareCondition(fieldName, Comparison.Smaller, value);
        }

        public static CompareCondition SmallerOrEqual(string fieldName, object value)
        {
            return new CompareCondition(fieldName, Comparison.SmallerOrEqual, value);
        }

        public static InCondition In(string fieldName, params object[] values)
        {
            return new InCondition(fieldName, values);
        }

        public static InCondition In(string fieldName, IEnumerable<object> values)
        {
            return new InCondition(fieldName, values);
        }
    }

    public static class ConditionsExtensions
    { 
        public static CompoundCondition And(this Condition a, Condition b)
        {
            return new CompoundCondition(a, b, Operator.And);
        }

        public static CompoundCondition Or(this Condition a, Condition b)
        {
            return new CompoundCondition(a, b, Operator.Or);
        }
    }

    public enum Comparison
    { 
        Equal,
        NotEqual,
        Smaller,
        SmallerOrEqual,
        Greater,
        GreaterOrEqual
    }

    public class CompareCondition : Condition
    {
        public string FieldName { get; private set; }
        public Comparison Comparison { get; private set; }
        public object Value { get; private set; }

        public CompareCondition(string fieldName, Comparison comparison, object value)
        {
            FieldName = fieldName;
            Comparison = comparison;
            Value = value;
        }

        public override bool Match(IDataObject obj)
        {
            var field = obj.Fields.Single(f => f.FieldName == FieldName);
            return field.Compare(Comparison, Value);
        }
    }

    public class InCondition : Condition
    {
        public string FieldName { get; private set; }
        public IEnumerable<object> Values { get; private set; }

        public InCondition(string fieldName, IEnumerable<object> values)
        {
            FieldName = fieldName;
            Values = values.ToList();
        }

        public override bool Match(IDataObject obj)
        {
            var field = obj.Fields.Single(f => f.FieldName == FieldName);
            return Values.Any(v => field.Compare(Comparison.Equal, v));
        }
    }

    public enum Operator
    {
        And,
        Or,
    }

    public class CompoundCondition : Condition
    { 
        public Condition A { get; private set; }
        public Condition B { get; private set; }
        public Operator Op { get; private set; }

        public CompoundCondition(Condition a, Condition b, Operator op)
        {
            A = a;
            B = b;
            Op = op;
        }

        public override bool Match(IDataObject obj)
        {
            switch (Op)
            {
                case Operator.And:
                    return A.Match(obj) && B.Match(obj);
                case Operator.Or:
                    return A.Match(obj) || B.Match(obj);
                default:
                    throw new NotSupportedException("Operator not supported");
            }
        }
    }
}
