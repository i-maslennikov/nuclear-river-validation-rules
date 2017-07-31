using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public class Reference
    {
        public static IEqualityComparer<Reference> Comparer = new ReferenceComparer();

        public Reference(int entityTypeId, long id)
            : this(entityTypeId, id, null)
        {
        }

        public Reference(int entityTypeId, long id, params Reference[] children)
        {
            Children = children ?? Array.Empty<Reference>();
            EntityType = entityTypeId;
            Id = id;
        }

        public int EntityType { get; }
        public long Id { get; }
        public IReadOnlyCollection<Reference> Children { get; }

        private class ReferenceComparer : IEqualityComparer<Reference>
        {
            public bool Equals(Reference x, Reference y)
                => x.EntityType == y.EntityType && x.Id == y.Id && x.Children.Count == y.Children.Count && x.Children.SequenceEqual(y.Children, this);

            public int GetHashCode(Reference obj)
                => obj.Id.GetHashCode() ^ obj.EntityType;
        }
    }

    public class Reference<TEntityType> : Reference
        where TEntityType : IdentityBase<TEntityType>, new()
    {
        private static readonly int EntityTypeId = EntityTypeBase<TEntityType>.Instance.Id;

        public Reference(long id)
            : base(EntityTypeId, id)
        {
        }

        public Reference(long id, params Reference[] children)
            : base(EntityTypeId, id, children)
        {
        }
    }
}
