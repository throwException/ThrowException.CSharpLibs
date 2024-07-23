using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public class DatabaseObject : IDataObject
    {
        private readonly List<DatabaseField> _fields;

        public Guid Id { get; private set; }
        public IEnumerable<DatabaseField> Fields { get { return _fields; } }
        public bool New { get; private set; } = true;
        public bool Modified { get { return Fields.Any(f => f.Modified); } }
        public DatabaseContext Context { get; private set; }

        IEnumerable<IDataField> IDataObject.Fields => _fields.Cast<IDataField>();

        public void EagerLoad(DatabaseContext context)
        {
            Context = context;
            foreach (var field in _fields)
            {
                field.EagerLoad();
            }
        }

        public DatabaseObject()
            : this(Guid.NewGuid())
        {
        }

        public DatabaseObject(Guid id, bool newObject = true)
        {
            Id = id;
            New = newObject;
            _fields = new List<DatabaseField>();

            var type = GetType();
            foreach (var property in type.GetProperties())
            {
                if (property.PropertyType.IsSubclassOf(typeof(DatabaseField)))
                {
                    var value = (DatabaseField)property.GetValue(this);
                    if (value == null)
                    {
                        value = (DatabaseField)property.PropertyType
                            .GetConstructor(new Type[] { })
                            .Invoke(new object[] { });
                        var field = property.GetBackingField();
                        field.SetValue(this, value);
                    }
                    value.Connect(property, this);
                    _fields.Add(value);

                    var defaultValueAttribute = (DefaultValueAttribute)type
                        .GetCustomAttributes(typeof(DefaultValueAttribute), true)
                        .SingleOrDefault();
                    if (defaultValueAttribute != null)
                    {
                        value.DatabaseValue = defaultValueAttribute.Value;
                    }
                }
            }
        }

        internal DatabaseObject ToList()
        {
            throw new NotImplementedException();
        }
    }
}
