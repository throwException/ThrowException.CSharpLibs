using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public interface IDatabase : IDisposable
    {
        ITransaction BeginTransaction();
        void Save(DatabaseObject obj, ITransaction transaction = null);
        void Delete<T>(Guid id, ITransaction transaction = null) where T : DatabaseObject, new();
        void Delete<T>(ITransaction transaction = null) where T : DatabaseObject, new();
        void Delete<T>(Condition condition, ITransaction transaction = null) where T : DatabaseObject, new();
        T Load<T>(Guid id, ITransaction transaction = null) where T : DatabaseObject, new();
        IEnumerable<T> Load<T>(ITransaction transaction = null) where T : DatabaseObject, new();
        IEnumerable<T> Load<T>(Condition condition, ITransaction transaction = null) where T : DatabaseObject, new();
        IEnumerable<Guid> List<T>(ITransaction transaction = null) where T : DatabaseObject, new();
        IEnumerable<Guid> List<T>(Condition condition, ITransaction transaction = null) where T : DatabaseObject, new();
        long Count<T>(ITransaction transaction = null) where T : DatabaseObject, new();
        long Count<T>(Condition condition, ITransaction transaction = null) where T : DatabaseObject, new();
        bool TableExists(string tableName, ITransaction transaction = null);
        void CreateTable<T>(ITransaction transaction = null) where T : DatabaseObject, new();
        void DropTable(string tableName, ITransaction transaction = null);
        void AddColumn<T>(Func<T, DatabaseField> selectField, ITransaction transaction = null) where T : DatabaseObject, new();
        void DropColumn<T>(string columnName, ITransaction transaction = null);
        void ModifyColumnType<T, V>(Func<T, DatabaseField> selectField, Func<DatabaseField, V, object> convertValue, ITransaction transaction = null) where T : DatabaseObject, new();
    }
}
