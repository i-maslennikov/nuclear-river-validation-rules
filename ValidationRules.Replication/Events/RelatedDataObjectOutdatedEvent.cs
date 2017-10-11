using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class RelatedDataObjectOutdatedEvent<TDataObjectId> : IEvent
        where TDataObjectId : struct
    {
        public Type DataObjectType { get; }
        public Type RelatedDataObjectType { get; }
        public TDataObjectId RelatedDataObjectId { get; }

        public RelatedDataObjectOutdatedEvent(Type dataObjectType, Type relatedDataObjectType, TDataObjectId relatedDataObjectId)
        {
            DataObjectType = dataObjectType;
            RelatedDataObjectType = relatedDataObjectType;
            RelatedDataObjectId = relatedDataObjectId;
        }

        private sealed class EqualityComparer : IEqualityComparer<RelatedDataObjectOutdatedEvent<TDataObjectId>>
        {
            public bool Equals(RelatedDataObjectOutdatedEvent<TDataObjectId> x, RelatedDataObjectOutdatedEvent<TDataObjectId> y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.DataObjectType == y.DataObjectType && x.RelatedDataObjectType == y.RelatedDataObjectType && x.RelatedDataObjectId.Equals(y.RelatedDataObjectId);
            }

            public int GetHashCode(RelatedDataObjectOutdatedEvent<TDataObjectId> obj)
            {
                unchecked
                {
                    var hashCode = obj.DataObjectType.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.RelatedDataObjectType.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.RelatedDataObjectId.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<RelatedDataObjectOutdatedEvent<TDataObjectId>> Comparer { get; } = new EqualityComparer();
    }
}