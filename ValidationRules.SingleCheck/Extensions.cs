using System.Linq;

namespace NuClear.ValidationRules.SingleCheck
{
    public static class Extensions
    {
        public static T[] Execute<T>(this IQueryable<T> queryable, string name = null)
        {
            return queryable.ToArray();
        }
    }
}
