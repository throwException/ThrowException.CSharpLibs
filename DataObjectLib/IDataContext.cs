using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.DataObjectLib
{
    public interface IDataContext : IDisposable
    {
        void BeginTransaction();
        void Commit();
        long Count<T>() where T : class, IDataObject, new();
        long Count<T>(Condition condition) where T : class, IDataObject, new();
        T Create<T>(Guid id) where T : class, IDataObject, new();
        T Create<T>() where T : class, IDataObject, new();
        void Delete<T>(Guid id) where T : class, IDataObject, new();
        void Delete<T>(Condition condition) where T : class, IDataObject, new();
        T Load<T>(Guid id) where T : class, IDataObject, new();
        IEnumerable<T> Load<T>() where T : class, IDataObject, new();
        IEnumerable<T> Load<T>(Condition condition) where T : class, IDataObject, new();
        void Rollback();
        void Save<T>(T obj) where T : class, IDataObject, new();
    }
}