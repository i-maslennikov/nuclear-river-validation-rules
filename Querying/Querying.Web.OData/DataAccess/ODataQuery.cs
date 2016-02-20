using System;
using System.Data.Entity;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Querying.Web.OData.DataAccess
{
    public class ODataQuery : IQuery, IDisposable
    {
        private readonly ODataDbContext _context;

        public ODataQuery(ODataDbContext context)
        {
            _context = context;
        }

        public IQueryable For(Type objType)
        {
            return _context.Set(objType).AsNoTracking();
        }

        public IQueryable<T> For<T>() where T : class
        {
            return _context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> For<T>(FindSpecification<T> findSpecification) where T : class
        {
            return _context.Set<T>().Where(findSpecification).AsNoTracking();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}