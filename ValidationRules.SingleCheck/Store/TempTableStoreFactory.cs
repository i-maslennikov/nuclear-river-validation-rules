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
    public sealed class TempTableStoreFactory : IStoreFactory
    {
        public static readonly Lazy<MappingSchema> MappingSchema =
            new Lazy<MappingSchema>(() => new MappingSchema(Schema.Facts, Schema.Aggregates));

        public static readonly Lazy<EqualityComparerFactory> EqualityComparerFactory =
            new Lazy<EqualityComparerFactory>(() => new EqualityComparerFactory(new LinqToDbPropertyProvider(MappingSchema.Value)));

        private readonly TempTableStore _tempTableStore;

        public TempTableStoreFactory(string connectionStringName)
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
            private readonly TempTableFactory _tempTableFactory;

            public TempTableStore(DataConnection connection)
            {
                _disposed = false;
                _connection = connection;
                _tempTableFactory = new TempTableFactory(connection);
            }

            void IStore.Add<T>(T entity)
            {
                _tempTableFactory.EnsureTempTableExist<T>();
                _connection.Insert(entity);
            }

            void IStore.AddRange<T>(IEnumerable<T> entities)
            {
                _tempTableFactory.EnsureTempTableExist<T>();
                _connection.BulkCopy(entities);
            }

            IQueryable<T> IQuery.For<T>()
            {
                _tempTableFactory.EnsureTempTableExist<T>();
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
                    _connection?.Dispose();
                }
            }
        }

        private class TempTableFactory
        {
            private readonly IDictionary<Type, string> _tableNames;
            private readonly DataConnection _connection;

            public TempTableFactory(DataConnection connection)
            {
                _tableNames = new Dictionary<Type, string>();
                _connection = connection;
            }

            public void EnsureTempTableExist<T>()
            {
                string name;
                if (!_tableNames.TryGetValue(typeof(T), out name))
                {
                    name = "#" + Guid.NewGuid().ToString("N");
                    _tableNames.Add(typeof(T), name);
                    _connection.MappingSchema.GetFluentMappingBuilder().Entity<T>().HasTableName(name);
                    _connection.CreateTable<T>();
                }
            }
        }
    }
}