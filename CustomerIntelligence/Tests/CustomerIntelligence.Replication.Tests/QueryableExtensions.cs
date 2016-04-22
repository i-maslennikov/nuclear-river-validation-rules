using System.Collections.Generic;
using System.Linq;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    static class QueryableExtensions
    {
        public static IEnumerable<T> ById<T>(this IQueryable<T> queryable, params long[] ids)
            where T: IIdentifiable<long>
        {
            return queryable.Where(new DefaultIdentityProvider().Create<T, long>(ids));
        }

        public static IEnumerable<T> ById<T>(this IQueryable<T> queryable, IEnumerable<long> ids)
            where T : IIdentifiable<long>
        {
            return queryable.Where(new DefaultIdentityProvider().Create<T, long>(ids));
        }
    }
}