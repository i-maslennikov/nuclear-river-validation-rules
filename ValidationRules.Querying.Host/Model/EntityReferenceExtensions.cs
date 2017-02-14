using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Querying.Host.Model
{
    public static class EntityReferenceCollection
    {
        public static NamedReference Get<T>(this IReadOnlyCollection<NamedReference> references)
            where T : IdentityBase<T>, new()
        {
            return references.First(x => string.Equals(x.Type, EntityTypeBase<T>.Instance.Description, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<NamedReference> GetMany<T>(this IReadOnlyCollection<NamedReference> references)
            where T : IdentityBase<T>, new()
        {
            return references.Where(x => string.Equals(x.Type, EntityTypeBase<T>.Instance.Description, StringComparison.OrdinalIgnoreCase));
        }
    }
}