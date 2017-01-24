using System;

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

        private bool Equals(RelatedDataObjectOutdatedEvent<TDataObjectId> other)
        {
            return DataObjectType == other.DataObjectType &&
                   RelatedDataObjectType == other.RelatedDataObjectType &&
                   RelatedDataObjectId.Equals(other.RelatedDataObjectId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            var a = obj as RelatedDataObjectOutdatedEvent<TDataObjectId>;
            return a != null && Equals(a);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DataObjectType.GetHashCode();
                hashCode = (hashCode * 397) ^ RelatedDataObjectType.GetHashCode();
                hashCode = (hashCode * 397) ^ RelatedDataObjectId.GetHashCode();
                return hashCode;
            }
        }
    }
}