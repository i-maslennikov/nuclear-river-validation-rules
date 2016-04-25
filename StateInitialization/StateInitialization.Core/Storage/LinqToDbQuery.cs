using System;
using System.Linq;

using LinqToDB.Data;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.StateInitialization.Core.Storage
{
    public sealed class LinqToDbQuery : IQuery
    {
        private readonly DataConnection _dataContext;

        public LinqToDbQuery(DataConnection dataContext)
        {
            _dataContext = dataContext;
        }

        public string LastQuery => _dataContext.LastQuery;

        public IQueryable For(Type objType)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> For<T>() where T : class
        {
            return _dataContext.GetTable<T>();
        }

        public IQueryable<T> For<T>(FindSpecification<T> findSpecification) where T : class
        {
            return _dataContext.GetTable<T>().Where(findSpecification);
        }
    }
}