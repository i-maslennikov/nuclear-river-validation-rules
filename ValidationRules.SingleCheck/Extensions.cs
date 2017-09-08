using System;
using System.Linq;

namespace NuClear.ValidationRules.SingleCheck
{
    public static class Extensions
    {
        public static T[] Execute<T>(this IQueryable<T> queryable, string name = null)
        {
            try
            {
                return queryable.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception while querying for {typeof(T).FullName}\n{queryable}", ex);
            }
        }
    }
}
