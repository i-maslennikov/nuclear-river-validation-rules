using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public static class ReferenceExtensions
    {
        public static Reference Get<T>(this IReadOnlyCollection<Reference> references)
            where T : IdentityBase<T>, new()
        {
            return references.First(x => x.EntityType == EntityTypeBase<T>.Instance.Id);
        }

        public static IEnumerable<Reference> GetMany<T>(this IReadOnlyCollection<Reference> references)
            where T : IdentityBase<T>, new()
        {
            return references.Where(x => x.EntityType == EntityTypeBase<T>.Instance.Id);
        }
    }
}