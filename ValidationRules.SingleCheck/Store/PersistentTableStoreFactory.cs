using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.Storage.Model.WebApp;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    /// <summary>
    /// Обеспечивает хранение в постоянных sql-таблицах с откатом всех изменений по завершению проверки.
    /// </summary>
    public sealed class PersistentTableStoreFactory : IStoreFactory
    {
        private readonly LockManager _lockManager;
        private readonly DataConnection _connection;
        private readonly Lock _lock;

        public PersistentTableStoreFactory(LockManager lockManager, SchemaManager schemaManager)
        {
            using (Probe.Create("Get lock"))
            {
                _lockManager = lockManager;
                _lock = _lockManager.GetLock();
                _connection = new DataConnection("Messages").AddMappingSchema(schemaManager.GetSchema(_lock));
                _connection.BeginTransaction(System.Data.IsolationLevel.Snapshot);
            }
        }

        public IStore CreateStore()
            => new Linq2DbStore(_connection);

        public IQuery CreateQuery()
            => new Linq2DbQuery(_connection);

        public void Dispose()
        {
            _connection.RollbackTransaction();
            _connection.Dispose();
            _lockManager.Release(_lock);
        }

        private class Linq2DbQuery : IQuery
        {
            private readonly DataConnection _connection;

            public Linq2DbQuery(DataConnection connection)
            {
                _connection = connection;
            }

            IQueryable<T> IQuery.For<T>()
            {
                return _connection.GetTable<T>();
            }

            IQueryable IQuery.For(Type objType)
            {
                throw new NotSupportedException();
            }

            IQueryable<T> IQuery.For<T>(FindSpecification<T> findSpecification)
            {
                throw new NotSupportedException();
            }
        }

        private class Linq2DbStore : IStore
        {
            private readonly DataConnection _connection;

            public Linq2DbStore(DataConnection connection)
            {
                _connection = connection;
            }

            void IStore.Add<T>(T entity)
            {
                _connection.Insert(entity);
            }

            void IStore.AddRange<T>(IEnumerable<T> entities)
            {
                _connection.GetTable<T>().BulkCopy(entities);
            }
        }
    }
}