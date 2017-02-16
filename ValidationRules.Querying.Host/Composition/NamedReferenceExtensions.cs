using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public static class NamedReferenceExtensions
    {
        public static NamedReference Get<T>(this IReadOnlyCollection<NamedReference> references)
            where T : IdentityBase<T>, new()
        {
            return references.First(x => x.Reference.EntityType == EntityTypeBase<T>.Instance.Id);
        }

        public static IEnumerable<NamedReference> GetMany<T>(this IReadOnlyCollection<NamedReference> references)
            where T : IdentityBase<T>, new()
        {
            return references.Where(x => x.Reference.EntityType == EntityTypeBase<T>.Instance.Id);
        }
    }
}