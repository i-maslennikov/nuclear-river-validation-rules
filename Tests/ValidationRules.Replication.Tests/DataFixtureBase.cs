using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Storage;

using NUnit.Framework;

namespace NuClear.ValidationRules.Replication.Tests
{
    internal abstract class DataFixtureBase
    {
        protected Store Store { get; private set; }

        [SetUp]
        public void FixtureBuildUp()
        {
            Store = new Store();
        }
    }

    internal sealed class Store
    {
        public Store()
        {
            var data = new HashSet<object>();

            EqualityComparerFactory = new EqualityComparerFactory(new LinqToDbPropertyProvider(Schema.Erm, Schema.Facts, Schema.Aggregates));

            Query = new StoreQuery(data);
            Builder = new StoreBuilder(data);
            RepositoryFactory = new StoreRepositoryFactory(data, EqualityComparerFactory);
        }

        public IQuery Query { get; }

        public StoreBuilder Builder { get; }

        public StoreRepositoryFactory RepositoryFactory { get; }

        public IEqualityComparerFactory EqualityComparerFactory { get; }

        public sealed class StoreRepositoryFactory
        {
            private readonly HashSet<object> _data;
            private readonly IEqualityComparerFactory _equalityComparerFactory;

            public StoreRepositoryFactory(HashSet<object> data, IEqualityComparerFactory equalityComparerFactory)
            {
                _data = data;
                _equalityComparerFactory = equalityComparerFactory;
            }

            public IBulkRepository<T> Create<T>() where T : class
            {
                var equalityComparer = _equalityComparerFactory.CreateIdentityComparer<T>();
                return new BulkRepository<T>(_data, equalityComparer);
            }

            private sealed class BulkRepository<T> : IBulkRepository<T>
            {
                private readonly HashSet<object> _data;
                private readonly IEqualityComparer<T> _equalityComparer;

                public BulkRepository(HashSet<object> data, IEqualityComparer<T> equalityComparer)
                {
                    _data = data;
                    _equalityComparer = equalityComparer;
                }

                public void Create(IEnumerable<T> objects)
                {
                    _data.UnionWith(objects.Cast<object>());
                }

                public void Update(IEnumerable<T> objects)
                {
                    var oldObjects = _data.OfType<T>().ToList();
                    var newObjects = objects.Union(oldObjects, _equalityComparer);

                    _data.ExceptWith(oldObjects.Cast<object>());
                    _data.UnionWith(newObjects.Cast<object>());
                }

                public void Delete(IEnumerable<T> objects)
                {
                    _data.ExceptWith(objects.Cast<object>());
                }
            }
        }

        public sealed class StoreBuilder
        {
            private readonly HashSet<object> _data;

            public StoreBuilder(HashSet<object> data)
            {
                _data = data;
            }

            public StoreBuilder Has<T>(T obj, params T[] moreObjects) where T : class
            {
                _data.Add(obj);
                if (moreObjects != null)
                {
                    _data.UnionWith(moreObjects);
                }
                return this;
            }
        }

        private sealed class StoreQuery : IQuery
        {
            private readonly HashSet<object> _data;

            public StoreQuery(HashSet<object> data)
            {
                _data = data;
            }

            public IQueryable For(Type objType)
            {
                return _data.Where(x => x.GetType() == objType).AsQueryable();
            }

            public IQueryable<T> For<T>() where T : class
            {
                return _data.OfType<T>().AsQueryable();
            }

            public IQueryable<T> For<T>(FindSpecification<T> findSpecification) where T : class
            {
                return _data.OfType<T>().AsQueryable().Where(findSpecification);
            }
        }
    }
}