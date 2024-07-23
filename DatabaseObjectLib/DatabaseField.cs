using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ThrowException.CSharpLibs.BytesUtilLib;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public enum LoadingBehavior
    {
        Eager,
        Lazy,
    }

    public enum LoadingStatus
    {
        None,
        Partial,
        Fully,
    }

    public abstract class DatabaseField : IDataField
    {
        internal PropertyInfo PropertyInfo { get; private set; }
        public string FieldName { get; private set; }
        internal DatabaseObject Object { get; private set; }
        public bool Modified { get; protected set; } = false;
        internal abstract bool IsInDatabase { get; }
        public abstract bool Nullable { get; }
        internal abstract Type DatabaseType { get; }
        internal abstract object DatabaseValue { get; set; }
        public abstract bool Compare(Comparison comparison, object value);
        internal abstract void EagerLoad();
        internal abstract Type ReferencedType { get; }
        public abstract LoadingStatus Status { get; }

        internal virtual void Connect(PropertyInfo propertyInfo, DatabaseObject obj)
        {
            PropertyInfo = propertyInfo;
            Object = obj;

            var columnNameAttribute = (ColumnNameAttribute)PropertyInfo
                .GetCustomAttributes(typeof(ColumnNameAttribute), true)
                .SingleOrDefault();
            if (columnNameAttribute == null)
            {
                FieldName = propertyInfo.Name;
            }
            else
            {
                FieldName = columnNameAttribute.Name;
            }
        }
    }

    public abstract class DatabaseField<T> : DatabaseField, IDataField<T>
    {
        public abstract T Value { get; set; }

        public string Name => FieldName;
    }

    public abstract class ReferenceBaseDatabaseField<T, U> : DatabaseField<T>
        where T : DatabaseObject, new()
        where U : DatabaseObject, new()
    {
        protected Guid? _valueId;
        protected T _value;
        protected readonly Func<T, ReferenceListField<U, T>> _listField = null;

        protected ReferenceBaseDatabaseField(Func<T, ReferenceListField<U, T>> referencedField)
        {
            _listField = referencedField;
        }

        internal override void Connect(PropertyInfo propertyInfo, DatabaseObject obj)
        {
            base.Connect(propertyInfo, obj);

            var loadingAttribute = (LoadingAttribute)PropertyInfo
                .GetCustomAttributes(typeof(LoadingAttribute), true)
                .SingleOrDefault();
            if (loadingAttribute != null)
            {
                Loading = loadingAttribute.Behavior;
            }
        }

        internal override Type ReferencedType => typeof(T);

        public override T Value
        {
            get
            {
                if ((_value == null) && (_valueId != null))
                {
                    _value = Object.Context.Load<T>(_valueId.Value);
                    if (_value != null)
                    {
                        if (_listField != null)
                        {
                            _listField(_value).Add((U)Object);
                        }
                        _valueId = null;
                    }
                }
                return _value;
            }
            set
            {
                if ((_value == null) && (_valueId != null))
                {
                    _value = Object.Context.Load<T>(_valueId.Value);
                    if (_value != null)
                    {
                        _valueId = null;
                    }
                }

                if ((_value != null) && (_listField != null))
                {
                    _listField(_value).Remove((U)Object);
                }

                Modified = true;
                _value = value;

                if ((_value != null) && (_listField != null))
                {
                    _listField(_value).Add((U)Object);
                }
            }
        }

        public LoadingBehavior Loading { get; private set; } = LoadingBehavior.Lazy;

        internal override bool IsInDatabase => true;

        internal override Type DatabaseType => typeof(Guid);

        public override LoadingStatus Status
        {
            get
            {
                if (_value != null)
                {
                    return LoadingStatus.Fully;
                }
                else if (_valueId != null)
                {
                    return LoadingStatus.None;
                }
                else
                {
                    return LoadingStatus.Fully;
                }
            }
        }

        public override bool Compare(Comparison comparison, object value)
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return ValueEquals(value);
                case Comparison.NotEqual:
                    return !ValueEquals(value);
                default:
                    throw new NotSupportedException("Comparison not supported on data type");
            }
        }

        private bool ValueEquals(object value)
        {
            if (_value != null)
            {
                return _value.Id.Equals(value);
            }
            else if (_valueId != null)
            {
                return _valueId.Value.Equals(value);
            }
            else
            {
                return value == null;
            }
        }

        internal override void EagerLoad()
        {
            if ((Loading == LoadingBehavior.Eager) && (_value == null) && (_valueId != null))
            {
                _value = Object.Context.Load<T>(_valueId.Value);
                if (_value != null)
                {
                    _valueId = null;
                }
                if ((_value != null) && (_listField != null))
                {
                    _listField(_value).Add((U)Object);
                }
            }
        }
    }

    public class ReferenceListField<T, U> : DatabaseField
        where T : DatabaseObject, new()
        where U : DatabaseObject, new()
    {
        private LoadingStatus _status;
        private readonly List<T> _list;
        private readonly string _referenceFieldName;
        private readonly Func<T, ReferenceBaseDatabaseField<U, T>> _referencedField;

        public IEnumerable<T> Values { get { return _list; } }

        internal override Type ReferencedType => null;

        public ReferenceListField(string referenceFieldName, Func<T, ReferenceBaseDatabaseField<U, T>> referencedField)
        {
            _referenceFieldName = referenceFieldName;
            _referencedField = referencedField;
            _list = new List<T>();
            _status = LoadingStatus.None;
        }

        internal override void Connect(PropertyInfo propertyInfo, DatabaseObject obj)
        {
            base.Connect(propertyInfo, obj);

            var loadingAttribute = (LoadingAttribute)PropertyInfo
                .GetCustomAttributes(typeof(LoadingAttribute), true)
                .SingleOrDefault();
            if (loadingAttribute != null)
            {
                Loading = loadingAttribute.Behavior;
            }
        }

        internal override bool IsInDatabase => false;

        public override bool Nullable => false;

        internal override Type DatabaseType => throw new NotImplementedException();

        internal override object DatabaseValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override LoadingStatus Status { get { return _status; } }

        public LoadingBehavior Loading { get; private set; } = LoadingBehavior.Lazy;

        public override bool Compare(Comparison comparison, object value)
        {
            throw new NotImplementedException();
        }

        internal override void EagerLoad()
        {
            if (Loading == LoadingBehavior.Eager)
            {
                Load();
            }
        }

        public void Load()
        {
            var list = Object.Context.Load<T>(
                new CompareCondition(_referenceFieldName, Comparison.Equal, Object.Id));
            foreach (var obj in list)
            {
                if (!_list.Contains(obj))
                {
                    _list.Add(obj);
                }
            }
            _status = LoadingStatus.Fully;
        }

        internal void Add(T value)
        {
            if (!_list.Contains(value))
            {
                _list.Add(value);
                if (_status == LoadingStatus.None)
                {
                    _status = LoadingStatus.Partial;
                }
            }
        }

        internal void Remove(T value)
        {
            if (_list.Contains(value))
            {
                _list.Remove(value);
            }
        }
    }

    public class ReferenceNotNullDatabaseField<T, U> : ReferenceBaseDatabaseField<T, U>
        where T : DatabaseObject, new()
        where U : DatabaseObject, new()
    {
        public ReferenceNotNullDatabaseField(Func<T, ReferenceListField<U, T>> referencedField)
            : base(referencedField)
        {
        }

        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                if (value == null)
                    throw new InvalidDataException("Null value not allowd");
                base.Value = value;
            }
        }

        public override bool Nullable => false;

        internal override object DatabaseValue
        {
            get
            {
                if (_value != null)
                {
                    return _value.Id;
                }
                else if (_valueId != null)
                {
                    return _valueId.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value == null)
                    throw new InvalidDataException("Null value not allowd");
                _value = null;
                _valueId = (Guid)value;
            }
        }
    }

    public class ReferenceDatabaseField<T, U> : ReferenceBaseDatabaseField<T, U>
        where T : DatabaseObject, new()
        where U : DatabaseObject, new()
    {
        public ReferenceDatabaseField(Func<T, ReferenceListField<U, T>> referencedField)
            : base(referencedField)
        {
        }

        public override bool Nullable => true;

        internal override object DatabaseValue
        {
            get
            {
                if (_value != null)
                {
                    return _value.Id;
                }
                else if (_valueId != null)
                {
                    return _valueId.Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _value = null;
                _valueId = (Guid)value;
            }
        }
    }

    public abstract class ScalarDatabaseField<T> : DatabaseField<T>
    {
        private T _value;

        internal override Type ReferencedType => null;

        public override bool Nullable
        {
            get
            {
                if (PropertyInfo.PropertyType.IsValueType)
                {
                    return false;
                }
                else if (PropertyInfo.PropertyType.IsSubclassOf(typeof(Nullable)))
                {
                    return true;
                }
                else
                {
                    var nullableAttribute = (NullableAttribute)PropertyInfo
                        .GetCustomAttributes(typeof(NullableAttribute), true)
                        .SingleOrDefault();
                    if (nullableAttribute != null)
                    {
                        return nullableAttribute.Value;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public override T Value
        {
            get
            {
                return _value;
            }
            set
            {
                Modified = true;
                _value = value;
            }
        }

        internal override bool IsInDatabase => true;

        internal override Type DatabaseType { get { return typeof(T); } }

        internal override object DatabaseValue
        {
            get => _value;
            set
            {
                _value = (T)value;
            }
        }

        protected static bool CompareValue<U>(U current, Comparison comparison, object value)
            where U : IComparable
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return current.CompareTo((U)value) == 0;
                case Comparison.NotEqual:
                    return current.CompareTo((U)value) != 0;
                case Comparison.Smaller:
                    return current.CompareTo((U)value) > 0;
                case Comparison.SmallerOrEqual:
                    return current.CompareTo((U)value) >= 0;
                case Comparison.Greater:
                    return current.CompareTo((U)value) < 0;
                case Comparison.GreaterOrEqual:
                    return current.CompareTo((U)value) <= 0;
                default:
                    throw new NotSupportedException("Comparison not supported on data type");
            }
        }

        protected static bool EqualValue<U>(U current, Comparison comparison, object value)
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return current.Equals((U)value);
                case Comparison.NotEqual:
                    return !current.Equals((U)value);
                default:
                    throw new NotSupportedException("Comparison not supported on data type");
            }
        }

        internal override void EagerLoad()
        {
        }

        public override LoadingStatus Status => LoadingStatus.Fully;
    }

    public class StringField : ScalarDatabaseField<string>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class GuidField : ScalarDatabaseField<Guid>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return EqualValue(Value, comparison, value);
        }
    }

    public class ByteField : ScalarDatabaseField<byte>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class SByteField : ScalarDatabaseField<sbyte>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class Int16Field : ScalarDatabaseField<short>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class UInt16Field : ScalarDatabaseField<ushort>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }

        internal override Type DatabaseType => typeof(int);

        internal override object DatabaseValue
        {
            get => Convert.ToInt32(base.DatabaseValue);
            set => base.DatabaseValue = Convert.ToUInt16(value);
        }
    }

    public class Int32Field : ScalarDatabaseField<int>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class UInt32Field : ScalarDatabaseField<uint>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }

        internal override Type DatabaseType => typeof(long);

        internal override object DatabaseValue
        {
            get => Convert.ToInt64(base.DatabaseValue);
            set => base.DatabaseValue = Convert.ToUInt32(value);
        }
    }

    public class Int64Field : ScalarDatabaseField<long>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class UInt64Field : ScalarDatabaseField<ulong>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }

        internal override Type DatabaseType => typeof(ulong);

        internal override object DatabaseValue
        {
            get => Convert.ToDecimal(base.DatabaseValue);
            set => base.DatabaseValue = Convert.ToUInt64(value);
        }
    }

    public class FloatField : ScalarDatabaseField<float>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class DoubleField : ScalarDatabaseField<double>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class DecimalField : ScalarDatabaseField<decimal>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class CharField : ScalarDatabaseField<char>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class BytesField : ScalarDatabaseField<byte[]>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return Value.AreEqual((byte[])value);
                case Comparison.NotEqual:
                    return !Value.AreEqual((byte[])value);
                default:
                    throw new NotSupportedException("Comparison not supported on data type");
            }
        }
    }

    public class BoolField : ScalarDatabaseField<bool>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return Value.Equals((bool)value);
                case Comparison.NotEqual:
                    return !Value.Equals((bool)value);
                default:
                    throw new NotSupportedException("Comparison not supported on data type");
            }
        }
    }

    public class DateTimeField : ScalarDatabaseField<DateTime>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }

    public class TimeSpanField : ScalarDatabaseField<TimeSpan>
    {
        public override bool Compare(Comparison comparison, object value)
        {
            return CompareValue(Value, comparison, value);
        }
    }
}
