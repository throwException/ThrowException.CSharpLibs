using System;
using System.Collections.Generic;
using System.Linq;
using ThrowException.CSharpLibs.DataObjectLib;

namespace ThrowException.CSharpLibs.DatabaseObjectLib
{
    public class DatabaseContext : IDataContext
    {
        private IDatabase _database;
        private ITransaction _transaction;
        private Dictionary<Guid, DatabaseObject> _cache;

        public DatabaseContext(IDatabase database)
        {
            _database = database;
            _cache = new Dictionary<Guid, DatabaseObject>();
        }

        public void BeginTransaction()
        {
            _transaction = _database.BeginTransaction();
        }

        public void Commit()
        {
            _transaction.Commit();
            _transaction = null;
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction = null;
        }

        private DatabaseObject DatabaseObject<T>(T obj)
            where T : class, IDataObject, new()
        {
            return obj as DatabaseObject;
        }

        private T TObject<T>(DatabaseObject obj)
            where T : class, IDataObject, new()
        {
            return obj as T;
        }

        public T Create<T>(Guid id)
            where T : class, IDataObject, new()
        {
            var obj = (T)typeof(T)
                .GetConstructor(new Type[] { typeof(Guid), typeof(bool) })
                .Invoke(new object[] { id, true });
            DatabaseObject(obj).EagerLoad(this);
            return obj;
        }

        public T Create<T>()
            where T : class, IDataObject, new()
        {
            var obj = new T();
            DatabaseObject(obj).EagerLoad(this);
            return obj;
        }

        public void Save<T>(T obj)
            where T : class, IDataObject, new()
        {
            if (DatabaseObject(obj).Context != this)
                throw new InvalidOperationException("Context mismatch");

            _database.Save(obj, _transaction);
            if (!_cache.ContainsKey(obj.Id))
            {
                _cache.Add(obj.Id, DatabaseObject(obj));
            }
        }

        public void Delete<T>(Guid id)
            where T : class, IDataObject, new()
        {
            _database.Delete<T>(id, _transaction);
            if (_cache.ContainsKey(id))
            {
                _cache.Remove(id);
            }
        }

        public void Delete<T>(Condition condition)
            where T : class, IDataObject, new()
        {
            _database.Delete<T>(condition, _transaction);
            foreach (var o in _cache.Values.Where(condition.Match).ToList())
            {
                _cache.Remove(o.Id);
            }
        }

        public T Load<T>(Guid id)
            where T : class, IDataObject, new()
        {
            if (_cache.ContainsKey(id))
            {
                return TObject<T>(_cache[id]);
            }
            else
            {
                T obj = _database.Load<T>(id, _transaction);
                if (obj != null)
                {
                    _cache.Add(obj.Id, DatabaseObject(obj));
                    DatabaseObject(obj).EagerLoad(this);
                }
                return obj;
            }
        }

        public IEnumerable<T> Load<T>()
            where T : class, IDataObject, new()
        {
            var local = _cache.Values.Where(o => o is T).ToList();
            var count = _database.Count<T>(_transaction);
            if (count == 0)
            {
                //return nothing
            }
            else if (count == local.Count)
            {
                foreach (var obj in local)
                {
                    yield return TObject<T>(obj);
                }
            }
            else if (!local.Any())
            {
                var list = _database.Load<T>(_transaction).ToList();
                foreach (var obj in list)
                {
                    _cache.Add(obj.Id, DatabaseObject(obj));
                }
                foreach (var obj in list)
                {
                    DatabaseObject(obj).EagerLoad(this);
                    yield return obj;
                }
            }
            else if (count <= 20)
            {
                var list = _database.Load<T>(_transaction).ToList();
                var newList = new List<T>();
                foreach (var obj in list)
                { 
                    if (_cache.ContainsKey(obj.Id))
                    {
                        yield return TObject<T>(_cache[obj.Id]);
                    }
                    else
                    {
                        newList.Add(obj);
                    }
                }
                foreach (var obj in newList)
                {
                    _cache.Add(obj.Id, DatabaseObject(obj));
                }
                foreach (var obj in newList)
                {
                    DatabaseObject(obj).EagerLoad(this);
                    yield return obj;
                }
            }
            else
            {
                var list = _database.List<T>(_transaction).ToList();
                foreach (var id in list)
                {
                    if (_cache.ContainsKey(id))
                    {
                        yield return TObject<T>(_cache[id]);
                        list.Remove(id);
                    }
                }
                while (list.Any())
                {
                    var subList = list.Take(10).ToList();
                    list.RemoveAll(id => subList.Contains(id));
                    var newList = _database.Load<T>(new InCondition("Id", subList.Cast<object>())).ToList();
                    foreach (var obj in newList)
                    {
                        _cache.Add(obj.Id, DatabaseObject(obj));
                    }
                    foreach (var obj in newList)
                    {
                        DatabaseObject(obj).EagerLoad(this);
                        yield return obj;
                    }
                }
            }
        }

        public IEnumerable<T> Load<T>(Condition condition)
            where T : class, IDataObject, new()
        {
            var local = _cache.Values.Where(o => (o is T) && condition.Match(o)).ToList();
            var count = _database.Count<T>(condition, _transaction);
            if (count == 0)
            {
                //return nothing
            }
            else if (count == local.Count)
            {
                foreach (var obj in local)
                {
                    yield return TObject<T>(obj);
                }
            }
            else if (!local.Any())
            {
                var list = _database.Load<T>(condition, _transaction).ToList();
                foreach (var obj in list)
                {
                    _cache.Add(obj.Id, DatabaseObject(obj));
                }
                foreach (var obj in list)
                {
                    DatabaseObject(obj).EagerLoad(this);
                    yield return obj;
                }
            }
            else if (count <= 20)
            {
                var list = _database.Load<T>(condition, _transaction).ToList();
                var newList = new List<T>();
                foreach (var obj in list)
                {
                    if (_cache.ContainsKey(obj.Id))
                    {
                        yield return TObject<T>(_cache[obj.Id]);
                    }
                    else
                    {
                        newList.Add(obj);
                    }
                }
                foreach (var obj in newList)
                {
                    _cache.Add(obj.Id, DatabaseObject(obj));
                }
                foreach (var obj in newList)
                {
                    DatabaseObject(obj).EagerLoad(this);
                    yield return obj;
                }
            }
            else
            {
                var list = _database.List<T>(condition, _transaction).ToList();
                foreach (var id in list)
                {
                    if (_cache.ContainsKey(id))
                    {
                        yield return TObject<T>(_cache[id]);
                        list.Remove(id);
                    }
                }
                while (list.Any())
                {
                    var subList = list.Take(10).ToList();
                    list.RemoveAll(id => subList.Contains(id));
                    var newList = _database.Load<T>(new InCondition("Id", subList.Cast<object>())).ToList();
                    foreach (var obj in newList)
                    {
                        _cache.Add(obj.Id, DatabaseObject(obj));
                    }
                    foreach (var obj in newList)
                    {
                        DatabaseObject(obj).EagerLoad(this);
                        yield return obj;
                    }
                }
            }
        }

        public long Count<T>()
            where T : class, IDataObject, new()
        {
            return _database.Count<T>(_transaction);
        }

        public long Count<T>(Condition condition)
            where T : class, IDataObject, new()
        {
            return _database.Count<T>(condition, _transaction);
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                Rollback();
            }
        }
    }
}
