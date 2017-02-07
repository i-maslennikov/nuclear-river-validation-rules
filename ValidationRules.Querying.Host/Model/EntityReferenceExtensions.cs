using System;
using System.Collections.Generic;
using System.Linq;

namespace NuClear.ValidationRules.Querying.Host.Model
{
    public static class EntityReferenceCollection
    {
        public static EntityReference Get(this IReadOnlyCollection<EntityReference> references, string entityType)
        {
            return references.Single(x => string.Equals(x.Type, entityType, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<EntityReference> GetMany(this IReadOnlyCollection<EntityReference> references, string entityType)
        {
            return references.Where(x => string.Equals(x.Type, entityType, StringComparison.OrdinalIgnoreCase));
        }
    }
}