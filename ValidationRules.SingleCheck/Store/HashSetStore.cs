using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.SingleCheck.Store
{
    public sealed class HashSetStore : IStore, IQuery
    {
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IDictionary<Type, IEnumerable> _store;

        public HashSetStore(IEqualityComparerFactory equalityComparerFactory)
        {
            _store = new Dictionary<Type, IEnumerable>();
            _equalityComparerFactory = equalityComparerFactory;
        }

        IQueryable IQuery.For(Type objType)
        {
            throw new NotImplementedException();
        }

        IQueryable<T> IQuery.For<T>()
        {
            return GetTable<T>().AsQueryable();
        }

        IQueryable<T> IQuery.For<T>(FindSpecification<T> findSpecification)
        {
            throw new NotImplementedException();
        }

        void IStore.Add<T>(T entity)
        {
            if (entity != null)
            {
                var table = GetTable<T>();
                table.Add(entity);
            }
        }

        void IStore.AddRange<T>(IEnumerable<T> entities)
        {
            var table = GetTable<T>();
            foreach (var entity in entities)
            {
                if (entity != null)
                {
                    table.Add(entity);
                }
            }
        }

        private HashSet<T> GetTable<T>()
        {
            IEnumerable table;
            if (_store.TryGetValue(typeof(T), out table))
            {
                return (HashSet<T>)table;
            }

            var collection = new HashSet<T>(_equalityComparerFactory.CreateCompleteComparer<T>());
            _store.Add(typeof(T), collection);
            return collection;
        }
    }
}