using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    /// <summary>
    /// Обеспечивает хранение в постоянных sql-таблицах с откатом всех изменений по завершению проверки.
    /// </summary>
    public sealed class Linq2DbTransactionStoreFactory : IStoreFactory
    {
        public static readonly Lazy<MappingSchema> MappingSchema =
            new Lazy<MappingSchema>(() => new MappingSchema(Schema.Facts, Schema.Aggregates));

        public static readonly Lazy<EqualityComparerFactory> EqualityComparerFactory =
            new Lazy<EqualityComparerFactory>(() => new EqualityComparerFactory(new LinqToDbPropertyProvider(MappingSchema.Value)));

        private readonly TempTableStore _tempTableStore;

        public Linq2DbTransactionStoreFactory(string connectionStringName)
        {
            _tempTableStore = new TempTableStore(new DataConnection(connectionStringName).AddMappingSchema(MappingSchema.Value));
        }

        public IStore CreateStore()
            => _tempTableStore;

        public IQuery CreateQuery()
            => _tempTableStore;

        private class TempTableStore : IStore, IQuery, IDisposable
        {
            private bool _disposed;
            private readonly DataConnection _connection;

            public TempTableStore(DataConnection connection)
            {
                _disposed = false;
                _connection = connection;
                _connection.BeginTransaction();
            }

            void IStore.Add<T>(T entity)
            {
                _connection.Insert(entity);
            }

            void IStore.AddRange<T>(IEnumerable<T> entities)
            {
                var x = entities.Distinct(EqualityComparerFactory.Value.CreateCompleteComparer<T>());
                _connection.BulkCopy(x);
            }

            IQueryable<T> IQuery.For<T>()
            {
                return _connection.GetTable<T>();
            }

            IQueryable IQuery.For(Type objType)
            {
                throw new NotImplementedException();
            }

            IQueryable<T> IQuery.For<T>(FindSpecification<T> findSpecification)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                if (disposing)
                {
                    _connection.RollbackTransaction();
                    _connection?.Dispose();
                }
            }
        }
    }
}