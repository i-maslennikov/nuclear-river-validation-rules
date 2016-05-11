using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Tests.MemoryDb
{
    internal sealed class MemoryMockQuery : IQuery
    {
        private readonly List<object> _data;

        public MemoryMockQuery(params object[] data)
        {
            _data = new List<object>(data);
        }

        public void Add(object data)
        {
            _data.Add(data);
        }

        public void AddRange(IEnumerable<object> data)
        {
            _data.AddRange(data);
        }

        IQueryable IQuery.For(Type objType)
        {
            return _data.Where(x => x.GetType() == objType).AsQueryable();
        }

        IQueryable<T> IQuery.For<T>()
        {
            return _data.OfType<T>().AsQueryable();
        }

        IQueryable<T> IQuery.For<T>(FindSpecification<T> findSpecification)
        {
            return _data.OfType<T>().AsQueryable().Where(findSpecification);
        }
    }
}