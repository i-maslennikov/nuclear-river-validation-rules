using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.Replication.Tests
{
    static class QueryableExtensions
    {
        public static IEnumerable<T> ById<T>(this IQueryable<T> queryable, params long[] ids)
            where T: IIdentifiable<long>
        {
            return queryable.Where(DefaultIdentityProvider.Instance.Create<T>(ids));
        }

        public static IEnumerable<T> ById<T>(this IQueryable<T> queryable, IEnumerable<long> ids)
            where T : IIdentifiable<long>
        {
            return queryable.Where(DefaultIdentityProvider.Instance.Create<T>(ids));
        }
    }
}