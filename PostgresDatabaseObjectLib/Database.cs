using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ThrowException.CSharpLibs.DatabaseObjectLib;
using Npgsql;
using NpgsqlTypes;
using ThrowException.CSharpLibs.BytesUtilLib;
using ThrowException.CSharpLibs.TypeParserLib;

namespace ThrowException.CSharpLibs.PostgresDatabaseObjectLib
{
    public class Database : IDatabase
    {
        public class TypeHandler
        {
            public Type Type { get; private set; }
            public object DefaultValue { get; private set; }
            public NpgsqlDbType DbType { get; private set; }
            public string DbTypeName { get; private set; }
            public Func<NpgsqlDataReader, int, object> ReadValue { get; private set; }

            public TypeHandler(Type type, NpgsqlDbType dbType, string dbTypeName, object defaultValue, Func<NpgsqlDataReader, int, object> readValue)
            {
                Type = type;
                DbType = dbType;
                DbTypeName = dbTypeName;
                DefaultValue = defaultValue;
                ReadValue = readValue;
            }
        }

        private readonly Dictionary<Type, TypeHandler> _types;
        private readonly NpgsqlConnection _connection;

        private uint _nextName = 0;

        public string CreateVariableName()
        {
            var name = LittleConverter.GetBytes(_nextName)
                .ToHexString()
                .Replace("0", "g")
                .Replace("1", "h")
                .Replace("2", "i")
                .Replace("3", "j")
                .Replace("4", "k")
                .Replace("5", "l")
                .Replace("6", "m")
                .Replace("7", "n")
                .Replace("8", "o")
                .Replace("9", "p");
            _nextName++;
            return "@" + name;
        }

        private void AddType(Type type, NpgsqlDbType dbType, string dbTypeName, object defaultValue, Func<NpgsqlDataReader, int, object> readValue)
        {
            _types.Add(type, new TypeHandler(type, dbType, dbTypeName, defaultValue, readValue));
        }

        public Database(string connectionString)
        {
            _types = new Dictionary<Type, TypeHandler>();
            AddType(typeof(char), NpgsqlDbType.Char, "char", '\0', (r, i) => r.GetChar(i));
            AddType(typeof(byte), NpgsqlDbType.Smallint, "smallint", (byte)0, (r, i) => r.GetByte(i));
            AddType(typeof(sbyte), NpgsqlDbType.Smallint, "smallint", (sbyte)0, (r, i) => (sbyte)r.GetInt16(i));
            AddType(typeof(short), NpgsqlDbType.Smallint, "smallint", (short)0, (r, i) => r.GetInt16(i));
            AddType(typeof(int), NpgsqlDbType.Integer, "integer", 0, (r, i) => r.GetInt32(i));
            AddType(typeof(long), NpgsqlDbType.Bigint, "bigint", 0L, (r, i) => r.GetInt64(i));
            AddType(typeof(Guid), NpgsqlDbType.Uuid, "uuid", Guid.Empty, (r, i) => r.GetGuid(i));
            AddType(typeof(bool), NpgsqlDbType.Boolean, "boolean", false, (r, i) => r.GetBoolean(i));
            AddType(typeof(byte[]), NpgsqlDbType.Bytea, "bytea", new byte[0], ReadBytes);
            AddType(typeof(TimeSpan), NpgsqlDbType.Interval, "interval", new TimeSpan(0), (r, i) => r.GetTimeSpan(i));
            AddType(typeof(DateTime), NpgsqlDbType.Timestamp, "timestamp", new DateTime(1970, 1, 1), (r, i) => r.GetDateTime(i));
            AddType(typeof(float), NpgsqlDbType.Real, "real", 0f, (r, i) => r.GetFloat(i));
            AddType(typeof(double), NpgsqlDbType.Double, "double precision", 0d, (r, i) => r.GetDouble(i));
            AddType(typeof(decimal), NpgsqlDbType.Numeric, "numeric(56, 28)", 0M, (r, i) => r.GetDecimal(i));
            AddType(typeof(ulong), NpgsqlDbType.Numeric, "numeric(20, 0)", 0UL, (r, i) => Convert.ToUInt64(r.GetValue(i)));
            AddType(typeof(string), NpgsqlDbType.Text, "text", "", (r, i) => r.GetString(i));

            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        public ITransaction BeginTransaction()
        {
            return new Transaction(_connection.BeginTransaction());
        }

        private string GetTableName(Type type, bool raw = false)
        {
            var tableNameAttribute = (TableNameAttribute)type
                .GetCustomAttributes(typeof(TableNameAttribute), true)
                .SingleOrDefault();
            if (tableNameAttribute == null)
            {
                if (raw)
                {
                    return type.Name.ToLowerInvariant();
                }
                else
                {
                    return string.Format("{0}", type.Name.ToLowerInvariant());
                }
            }
            else
            {
                if (raw)
                {
                    return tableNameAttribute.Name.ToLowerInvariant();
                }
                else
                {
                    return string.Format("{0}", tableNameAttribute.Name.ToLowerInvariant());
                }
            }
        }

        private string GetColumnName(DatabaseField field)
        {
            return GetColumnName(field, false);
        }

        private string GetColumnName(DatabaseField field, bool raw)
        {
            if (raw)
            {
                return field.FieldName.ToLowerInvariant();
            }
            else
            {
                return string.Format("{0}", field.FieldName.ToLowerInvariant());
            }
        }

        private NpgsqlTransaction ToTransaction(ITransaction transaction)
        {
            return (transaction as Transaction)?.Base;
        }

        public void Delete<T>(ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var text = string.Format(
                "DELETE FROM {0}", 
                GetTableName(typeof(T)));
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.ExecuteNonQuery();
        }

        public void Delete<T>(Guid id, ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var variableName = CreateVariableName();
            var text = string.Format(
                "DELETE FROM {0} WHERE id = {1}", 
                GetTableName(typeof(T)),
                variableName);
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.Parameters.Add(new NpgsqlParameter(variableName, id));
            command.ExecuteNonQuery();
        }

        public void Delete<T>(Condition condition, ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var dbCondition = ConvertCondition<T>(condition);
            var text = string.Format("DELETE FROM {0} WHERE {1}", GetTableName(typeof(T)), dbCondition.Text);
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.Parameters.AddRange(dbCondition.Parameters.ToArray());
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        private T CreateObjectWidthId<T>(Guid id)
        {
            return (T)typeof(T)
                .GetConstructor(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(Guid), typeof(bool) }, null)
                .Invoke(new object[] { id, false });
        }

        private TypeHandler GetTypeHandler(Type type)
        {
            if (_types.ContainsKey(type))
            {
                return _types[type];
            }
            else
            {
                string message = string.Format(
                    "Database provider {0} does not support type {1}",
                    GetType().FullName,
                    type.FullName);
                throw new NotSupportedException(message);
            }
        }

        private static byte[] ReadBytes(NpgsqlDataReader reader, int index)
        {
            using (var dbStream = reader.GetStream(index))
            {
                using (var memory = new MemoryStream())
                {
                    var buffer = new byte[4096];
                    var count = 1;
                    while (count > 0)
                    {
                        count = dbStream.Read(buffer, 0, buffer.Length);
                        memory.Write(buffer, 0, count);
                    }
                    return memory.ToArray();
                }
            }
        }

        public T Load<T>(Guid id, ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var prototype = new T();
            var text = string.Format(
                "SELECT {0} FROM {1} WHERE id = @id",
                string.Join(", ", prototype.Fields.Where(f => f.IsInDatabase).Select(GetColumnName)),
                GetTableName(typeof(T)));
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.Parameters.Add(new NpgsqlParameter("@id", id));
            using (var reader = command.ExecuteReader())
            { 
                if (reader.Read())
                {
                    var obj = CreateObjectWidthId<T>(id);
                    int index = 0;
                    foreach (var field in obj.Fields.Where(f => f.IsInDatabase))
                    {
                        AssignDatabaseValue(reader, index, field);
                        index++;
                    }
                    return obj;
                }
            }
            return null;
        }

        public IEnumerable<T> Load<T>(ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var prototype = new T();
            var text = string.Format(
                "SELECT id, {0} FROM {1}",
                string.Join(", ", prototype.Fields.Where(f => f.IsInDatabase).Select(GetColumnName)),
                GetTableName(typeof(T)));
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var obj = CreateObjectWidthId<T>(reader.GetGuid(0));
                    int index = 1;
                    foreach (var field in obj.Fields.Where(f => f.IsInDatabase))
                    {
                        AssignDatabaseValue(reader, index, field);
                        index++;
                    }
                    yield return obj;
                }
            }
        }

        public IEnumerable<T> Load<T>(Condition condition, ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var prototype = new T();
            var dbCondition = ConvertCondition<T>(condition);
            var text = string.Format(
                "SELECT id, {0} FROM {1} WHERE {2}",
                string.Join(", ", prototype.Fields.Where(f => f.IsInDatabase).Select(GetColumnName)),
                GetTableName(typeof(T)),
                dbCondition.Text);
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.Parameters.AddRange(dbCondition.Parameters.ToArray());
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var obj = CreateObjectWidthId<T>(reader.GetGuid(0));
                    int index = 1;
                    foreach (var field in obj.Fields.Where(f => f.IsInDatabase))
                    {
                        AssignDatabaseValue(reader, index, field);
                        index++;
                    }
                    yield return obj;
                }
            }
        }

        private void AssignDatabaseValue(NpgsqlDataReader reader, int index, DatabaseField field)
        {
            if (reader.IsDBNull(index))
            {
                field.DatabaseValue = null;
            }
            else
            {
                field.DatabaseValue = GetTypeHandler(field.DatabaseType).ReadValue(reader, index);
            }
        }

        public void Save(DatabaseObject obj, ITransaction transaction = null)
        {
            if (obj.New)
            {
                var names = new Dictionary<DatabaseField, string>();
                var fields = obj.Fields.Where(f => f.IsInDatabase).ToList();
                foreach (var field in fields)
                {
                    names.Add(field, CreateVariableName());
                }
                var idVariableName = CreateVariableName();
                var text = string.Format(
                    "INSERT INTO {0} (Id, {1}) VALUES ({2}, {3})",
                    GetTableName(obj.GetType()),
                    string.Join(", ", fields.Select(GetColumnName)),
                    idVariableName,
                    string.Join(", ", names.Values));
                var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
                command.Parameters.Add(new NpgsqlParameter(idVariableName, obj.Id));
                foreach (var field in fields)
                {
                    var parameter = new NpgsqlParameter(names[field], GetTypeHandler(field.DatabaseType).DbType);
                    if (field.DatabaseValue == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = field.DatabaseValue;
                    }
                    command.Parameters.Add(parameter);
                }
                command.ExecuteNonQuery();
            }
            else if (obj.Modified)
            {
                var names = new Dictionary<DatabaseField, string>();
                foreach (var field in obj.Fields.Where(f => f.IsInDatabase && f.Modified))
                {
                    names.Add(field, CreateVariableName());
                }
                var text = string.Format(
                    "UPDATE {0} SET {1} WHERE id = @id",
                    GetTableName(obj.GetType()),
                    string.Join(", ", obj.Fields.Where(f => f.IsInDatabase).Select(f => string.Format("{0} = {1}", GetColumnName(f), names[f]))));
                var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
                command.Parameters.Add(new NpgsqlParameter("@id", obj.Id));
                foreach (var field in obj.Fields.Where(f => f.IsInDatabase && f.Modified))
                {
                    var parameter = new NpgsqlParameter(names[field], GetTypeHandler(field.DatabaseType).DbType);
                    if (field.DatabaseValue == null)
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = field.DatabaseValue;
                    }
                    command.Parameters.Add(parameter);
                }
                command.ExecuteNonQuery();
            }
        }

        private string ColumnPreTypeDefinition(DatabaseField field)
        {
            var handler = GetTypeHandler(field.DatabaseType);
            return handler.DbTypeName;
        }

        private string ColumnTypeDefinition(DatabaseField field)
        {
            var preType = ColumnPreTypeDefinition(field);

            if (!field.Nullable)
            {
                preType += " NOT NULL";
            }

            return preType;
        }

        private string ColumnDefinition(DatabaseField field)
        {
            var fullType = ColumnTypeDefinition(field);

            if (field.FieldName.ToLowerInvariant() == "id")
            {
                fullType += " PRIMARY KEY";
            }

            return string.Format("{0} {1}", GetColumnName(field), fullType);
        }

        private class Column
        { 
            public string Name { get; private set; }
            public string DataType { get; private set; }
            public bool IsNullable { get; private set; }

            public Column(string name, string dataType, bool isNullable)
            {
                Name = name;
                DataType = dataType;
                IsNullable = isNullable;
            }
        }

        private IEnumerable<Column> ReadColumns(string tableName, ITransaction transaction = null)
        {
            var columnsText = string.Format(
                "SELECT column_name, data_type, is_nullable FROM information_schema.columns WHERE table_schema = 'public' AND table_name = {0}",
                tableName);
            var columnsCommand = new NpgsqlCommand(columnsText, _connection, ToTransaction(transaction));
            using (var columnsReader = columnsCommand.ExecuteReader())
            {
                while (columnsReader.Read())
                {
                    var columnName = columnsReader.GetString(0);
                    var dataType = columnsReader.GetString(1);
                    var isNullable = columnsReader.GetBoolean(2);
                    yield return new Column(columnName, dataType, isNullable);
                }
            }
        }

        public bool TableExists(string tableName, ITransaction transaction = null)
        {
            var command = new NpgsqlCommand("SELECT count(1) FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = @tablename", _connection, ToTransaction(transaction));
            command.Parameters.Add(new NpgsqlParameter("@tablename", tableName));
            return (long)command.ExecuteScalar() == 1;
        }

        private string ForeignKey(DatabaseField field)
        {
            if (field.ReferencedType != null)
            {
                var name = string.Format(
                    "fk_{0}_{1}",
                    GetColumnName(field),
                    GetTableName(field.ReferencedType, true));
                return string.Format(
                    "CONSTRAINT {0} FOREIGN KEY ({1}) REFERENCES {2}(id)",
                    name,
                    GetColumnName(field),
                    GetTableName(field.ReferencedType));
            }
            else
            {
                return null;
            }
        }

        public void CreateTable<T>(ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var temp = (DatabaseObject)typeof(T)
                .GetConstructor(new Type[] { })
                .Invoke(new object[] { });
            var definitions = temp.Fields.Where(f => f.IsInDatabase).Select(ColumnDefinition).ToList();
            definitions.AddRange(temp.Fields.Select(ForeignKey).Where(fk => fk != null));
            var columns = string.Join(", ",
                definitions);
            var text = string.Format(
                "CREATE TABLE {0} (id uuid PRIMARY KEY, {1})",
                GetTableName(typeof(T)),
                columns);
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.ExecuteNonQuery();
        }

        public void DropTable(string tableName, ITransaction transaction = null)
        {
            var text = string.Format(
                "DROP TABLE {0} CASCADE",
                tableName);
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.ExecuteNonQuery();
        }

        private object GetDefaultValue(DatabaseField field)
        {
            var defaultValueAttribute = (DefaultValueAttribute)field.PropertyInfo
                .GetCustomAttributes(typeof(DefaultValueAttribute), true)
                .SingleOrDefault();
            if (defaultValueAttribute != null)
            {
                if (defaultValueAttribute.Value.GetType() != field.DatabaseType)
                {
                    return defaultValueAttribute.Value;
                }
                else
                {
                    var parameters = new object[] { defaultValueAttribute.Value.ToString(), null };
                    var result = (bool)typeof(TypeParsers)
                        .GetMethod("TryParse")
                        .MakeGenericMethod(field.DatabaseType)
                        .Invoke(null, parameters);
                    if (result)
                    {
                        return parameters[1];
                    }
                    else
                    {
                        throw new InvalidDataException("Default value could not be converted");
                    }
                }
            }
            else
            {
                return GetTypeHandler(field.DatabaseType).DefaultValue;
            }
        }

        public void AddColumn<T>(Func<T, DatabaseField> selectField, ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var temp = new T();
            var field = selectField(temp);
            var tableName = GetTableName(typeof(T));
            if (field.Nullable)
            {
                string addText = string.Format(
                    "ALTER TABLE {0} ADD COLUMN {1}",
                    tableName,
                    ColumnDefinition(field));
                var addCommand = new NpgsqlCommand(addText, _connection, ToTransaction(transaction));
                addCommand.ExecuteNonQuery();
            }
            else
            {
                string addText = string.Format(
                    "ALTER TABLE {0} ADD COLUMN {1}",
                    tableName,
                    ColumnDefinition(field));
                var addCommand = new NpgsqlCommand(addText, _connection, ToTransaction(transaction));
                addCommand.ExecuteNonQuery();

                var updateName = CreateVariableName();
                string updateText = string.Format(
                    "UPDATE {0} SET {1} = {2}",
                    tableName,
                    GetColumnName(field),
                    updateName);
                var updateCommand = new NpgsqlCommand(updateName, _connection, ToTransaction(transaction));
                updateCommand.Parameters.Add(new NpgsqlParameter(updateName, GetDefaultValue(field)));
                updateCommand.ExecuteNonQuery();

                var notNullText = string.Format(
                    "ALTER TABLE {0} ALTER COLUMN {1} SET NOT NULL",
                    tableName,
                    GetColumnName(field));
                var notNullCommand = new NpgsqlCommand(notNullText, _connection, ToTransaction(transaction));
                notNullCommand.ExecuteNonQuery();
            }

            var foreignKey = ForeignKey(field);
            if (foreignKey != null)
            {
                var foreignKeyText = string.Format(
                    "ALTER TABLE {0} ADD {1}",
                    tableName,
                    foreignKey);
                var foreignKeyCommand = new NpgsqlCommand(foreignKeyText, _connection, ToTransaction(transaction));
                foreignKeyCommand.ExecuteNonQuery();
            }
        }

        public void DropColumn<T>(string columnName, ITransaction transaction = null)
        {
            var text = string.Format(
                "ALTER TABLE {0} DROP COLUMN '{1}' CASCADE",
                GetTableName(typeof(T)),
                columnName);
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.ExecuteNonQuery();
        }

        public void ModifyColumnType<T, V>(Func<T, DatabaseField> selectField, Func<DatabaseField, V, object> convertValue, ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var tableName = GetTableName(typeof(T));
            var temp = new T();
            var field = selectField(temp);
            var columnName = GetColumnName(field);

            var typeText = string.Format(
                "ALTER TABLE {0} ALTER COLUMN {1} TYPE {2}",
                tableName,
                columnName,
                ColumnPreTypeDefinition(field));
            var command = new NpgsqlCommand(typeText, _connection, ToTransaction(transaction));
            command.ExecuteNonQuery();

            if (field.Nullable)
            {
                var notNullText = string.Format(
                    "ALTER TABLE {0} ALTER COLUMN {1} DROP NOT NULL",
                    tableName,
                    columnName);
                var notNull = new NpgsqlCommand(notNullText, _connection, ToTransaction(transaction));
                notNull.ExecuteNonQuery();
            }
            else
            {
                var notNullText = string.Format(
                    "ALTER TABLE {0} ALTER COLUMN {1} SET NOT NULL",
                    tableName,
                    columnName);
                var notNull = new NpgsqlCommand(notNullText, _connection, ToTransaction(transaction));
                notNull.ExecuteNonQuery();
            }
        }

        public IEnumerable<Guid> List<T>(ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var prototype = new T();
            var text = string.Format(
                "SELECT id FROM {0}",
                GetTableName(typeof(T)));
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    yield return reader.GetGuid(0);
                }
            }
        }

        public IEnumerable<Guid> List<T>(Condition condition, ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var prototype = new T();
            var dbCondition = ConvertCondition<T>(condition);
            var text = string.Format(
                "SELECT id FROM {0} WHERE {1}",
                GetTableName(typeof(T)),
                dbCondition.Text);
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.Parameters.AddRange(dbCondition.Parameters.ToArray());
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    yield return reader.GetGuid(0);
                }
            }
        }

        private string ConvertOperator(Operator op)
        {
            switch (op)
            {
                case Operator.And:
                    return "AND";
                case Operator.Or:
                    return "OR";
                default:
                    throw new NotSupportedException("Operator not supported by database");
            }
        }

        private string ConvertComparison(Comparison comparison)
        {
            switch (comparison)
            {
                case Comparison.Equal:
                    return "=";
                case Comparison.NotEqual:
                    return "!=";
                case Comparison.Smaller:
                    return "<";
                case Comparison.SmallerOrEqual:
                    return "<=";
                case Comparison.Greater:
                    return ">";
                case Comparison.GreaterOrEqual:
                    return ">=";
                default:
                    throw new NotSupportedException("Comparison not supported by database");
            }
        }

        public DbCondition ConvertCondition<T>(Condition condition)
             where T : DatabaseObject, new()
        { 
            if (condition is CompareCondition compareCondition)
            {
                var temp = new T();
                var field = temp.Fields.Single(f => f.FieldName == compareCondition.FieldName);
                var columnName = GetColumnName(field);
                var variableName = CreateVariableName();
                var text = string.Format(
                    "({0} {1} {2})",
                    columnName,
                    ConvertComparison(compareCondition.Comparison),
                    variableName);
                return new DbCondition(text, new NpgsqlParameter(variableName, compareCondition.Value));
            }
            else if (condition is CompoundCondition compoundCondition)
            {
                var conditionA = ConvertCondition<T>(compoundCondition.A);
                var conditionB = ConvertCondition<T>(compoundCondition.B);
                var text = string.Format(
                    "({0} {1} {2})",
                    conditionA.Text,
                    ConvertOperator(compoundCondition.Op),
                    conditionB.Text);
                var parameters = new List<NpgsqlParameter>();
                parameters.AddRange(conditionA.Parameters);
                parameters.AddRange(conditionB.Parameters);
                return new DbCondition(text, parameters);
            }
            else
            {
                throw new NotSupportedException("Comparison not supported by database");
            }
        }

        public long Count<T>(ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var prototype = new T();
            var text = string.Format(
                "SELECT count(id) FROM {0}",
                GetTableName(typeof(T)));
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            return (long)command.ExecuteScalar();
        }

        public long Count<T>(Condition condition, ITransaction transaction = null) where T : DatabaseObject, new()
        {
            var prototype = new T();
            var dbCondition = ConvertCondition<T>(condition);
            var text = string.Format(
                "SELECT count(id) FROM {0} WHERE {1}",
                GetTableName(typeof(T)),
                dbCondition.Text);
            var command = new NpgsqlCommand(text, _connection, ToTransaction(transaction));
            command.Parameters.AddRange(dbCondition.Parameters.ToArray());
            return (long)command.ExecuteScalar();
        }
    }

    public class DbCondition
    {
        private readonly List<NpgsqlParameter> _parameters;

        public string Text { get; private set; }
        public IEnumerable<NpgsqlParameter> Parameters { get { return _parameters; } }

        public DbCondition(string text, params NpgsqlParameter[] parameters)
        {
            Text = text;
            _parameters = parameters.ToList();
        }

        public DbCondition(string text, IEnumerable<NpgsqlParameter> parameters)
        {
            Text = text;
            _parameters = parameters.ToList();
        }
    }

    public class Transaction : ITransaction
    {
        public NpgsqlTransaction Base { get; private set; }

        public Transaction(NpgsqlTransaction transaction)
        {
            Base = transaction;
        }

        public void Commit()
        {
            Base.Commit();
        }

        public void Dispose()
        {
            Base.Dispose();
        }

        public void Rollback()
        {
            Base.Rollback();
        }
    }
}
