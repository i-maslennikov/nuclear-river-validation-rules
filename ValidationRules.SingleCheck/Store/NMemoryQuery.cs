using System;
using System.Collections.Generic;
using System.Linq;

using NMemory.Modularity;
using NMemory.Tables;

using NuClear.Replication.Core;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public sealed class NMemoryQuery : IStore, IQuery
    {
        private readonly IDatabase _database;
        private readonly EqualityComparerFactory _equalityComparerFactory;
        private readonly INMemoryTableRegistrar _tableRegistrar;

        public NMemoryQuery(IDatabase database, INMemoryTableRegistrar tableRegistrar, EqualityComparerFactory equalityComparerFactory)
        {
            _database = database;
            _tableRegistrar = tableRegistrar;
            _equalityComparerFactory = equalityComparerFactory;
        }

        IQueryable IQuery.For(Type objType)
        {
            throw new NotImplementedException();
        }

        IQueryable<T> IQuery.For<T>()
        {
            return GetTable<T>();
        }

        IQueryable<T> IQuery.For<T>(FindSpecification<T> findSpecification)
        {
            return GetTable<T>().Where(findSpecification);
        }

        void IStore.Add<T>(T entity)
        {
            var t = GetTable<T>();
            t.Insert(entity);
        }

        void IStore.AddRange<T>(IEnumerable<T> entities)
        {
            // Костыль из-за того, что в NMemory distinct, union возвращает дубли. Вроде, помогает.
            entities = entities.Distinct(_equalityComparerFactory.CreateCompleteComparer<T>());

            var table = GetTable<T>();
            foreach (var entity in entities)
            {
                table.Insert(entity);
            }
        }

        private ITable<T> GetTable<T>() where T : class
            => FindTable<T>() ?? _tableRegistrar.RegisterTable<T>(_database);

        private ITable<T> FindTable<T>() where T : class
            => (ITable<T>)_database.Tables.FindTable(typeof(T));
    }
}
