using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public class ReferenceComparer : IEqualityComparer<Reference>
    {
        public static readonly IEqualityComparer<Reference> Instance = new ReferenceComparer();

        public bool Equals(Reference x, Reference y)
            => x.EntityType == y.EntityType && x.Id == y.Id;

        public int GetHashCode(Reference obj)
            => obj.Id.GetHashCode() ^ obj.EntityType;
    }
}