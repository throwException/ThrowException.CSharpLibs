using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public interface IDatabaseContext
    {
        void BeginTransaction();
        void Commit();
        long Count<T>() where T : DatabaseObject, new();
        long Count<T>(Condition condition) where T : DatabaseObject, new();
        T Create<T>(Guid id) where T : DatabaseObject, new();
        T Create<T>() where T : DatabaseObject, new();
        void Delete<T>(Guid id) where T : DatabaseObject, new();
        void Delete<T>(Condition condition) where T : DatabaseObject, new();
        void Dispose();
        T Load<T>(Guid id) where T : DatabaseObject, new();
        IEnumerable<T> Load<T>() where T : DatabaseObject, new();
        IEnumerable<T> Load<T>(Condition condition) where T : DatabaseObject, new();
        void Rollback();
        void Save(DatabaseObject obj);
    }
}