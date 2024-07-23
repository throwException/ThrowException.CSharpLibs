using System;
using System.Collections.Generic;
using System.Reflection;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.DataObjectLib
{
    public interface IDataProvider : IDisposable
    {
        IDataContext CreateContext();
        ITransaction BeginTransaction();
        void Save<T>(T obj, ITransaction transaction = null) where T : class, IDataObject, new();
        void Delete<T>(Guid id, ITransaction transaction = null) where T : class, IDataObject, new();
        void Delete<T>(ITransaction transaction = null) where T : class, IDataObject, new();
        void Delete<T>(Condition condition, ITransaction transaction = null) where T : class, IDataObject, new();
        T Load<T>(Guid id, ITransaction transaction = null) where T : class, IDataObject, new();
        IEnumerable<T> Load<T>(ITransaction transaction = null) where T : class, IDataObject, new();
        IEnumerable<T> Load<T>(Condition condition, ITransaction transaction = null) where T : class, IDataObject, new();
        IEnumerable<Guid> List<T>(ITransaction transaction = null) where T : class, IDataObject, new();
        IEnumerable<Guid> List<T>(Condition condition, ITransaction transaction = null) where T : class, IDataObject, new();
        long Count<T>(ITransaction transaction = null) where T : class, IDataObject, new();
        long Count<T>(Condition condition, ITransaction transaction = null) where T : class, IDataObject, new();
    }
}
