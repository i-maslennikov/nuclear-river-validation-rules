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
    public sealed class PersistentTableStoreFactory : IStoreFactory
    {
        public static readonly Lazy<MappingSchema> MappingSchema =
            new Lazy<MappingSchema>(() => new MappingSchema(Schema.Facts, Schema.Aggregates));

        public static readonly Lazy<EqualityComparerFactory> EqualityComparerFactory =
            new Lazy<EqualityComparerFactory>(() => new EqualityComparerFactory(new LinqToDbPropertyProvider(MappingSchema.Value)));

        private readonly TableStore _tableStore;

        public PersistentTableStoreFactory(string connectionStringName)
        {
            _tableStore = new TableStore(new DataConnection(connectionStringName).AddMappingSchema(MappingSchema.Value));
        }

        public IStore CreateStore()
            => _tableStore;

        public IQuery CreateQuery()
            => _tableStore;

        private class TableStore : IStore, IQuery, IDisposable
        {
            private bool _disposed;
            private readonly DataConnection _connection;

            public TableStore(DataConnection connection)
            {
                _disposed = false;
                _connection = connection;
                _connection.BeginTransaction();
            }

            void IStore.Add<T>(T entity)
            {
                var original = _connection.MappingSchema.GetAttribute<TableAttribute>(typeof(T));
                _connection.Insert(entity, schemaName: "dbo", tableName: $"{original.Schema}_{original.Name ?? typeof(T).Name}");
            }

            void IStore.AddRange<T>(IEnumerable<T> entities)
            {
                var original = _connection.MappingSchema.GetAttribute<TableAttribute>(typeof(T));
                var table = _connection.GetTable<T>().SchemaName("dbo").TableName($"{original.Schema}_{original.Name ?? typeof(T).Name}");
                table.BulkCopy(entities);
            }

            IQueryable<T> IQuery.For<T>()
            {
                var original = _connection.MappingSchema.GetAttribute<TableAttribute>(typeof(T));
                return _connection.GetTable<T>().SchemaName("dbo").TableName($"{original.Schema}_{original.Name ?? typeof(T).Name}");
            }

            IQueryable IQuery.For(Type objType)
            {
                throw new NotSupportedException();
            }

            IQueryable<T> IQuery.For<T>(FindSpecification<T> findSpecification)
            {
                throw new NotSupportedException();
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