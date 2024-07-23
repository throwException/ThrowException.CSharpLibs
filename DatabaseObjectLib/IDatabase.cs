using System;
using System.Collections.Generic;
using System.Reflection;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public interface IDatabase : IDataProvider
    {
        bool TableExists(string tableName, ITransaction transaction = null);
        void CreateTable<T>(ITransaction transaction = null) where T : DatabaseObject, new();
        void DropTable(string tableName, ITransaction transaction = null);
        void AddColumn<T>(Func<T, DatabaseField> selectField, ITransaction transaction = null) where T : DatabaseObject, new();
        void DropColumn<T>(string columnName, ITransaction transaction = null);
        void ModifyColumnType<T, V>(Func<T, DatabaseField> selectField, Func<DatabaseField, V, object> convertValue, ITransaction transaction = null) where T : DatabaseObject, new();
    }
}
