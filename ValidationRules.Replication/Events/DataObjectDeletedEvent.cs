using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class DataObjectDeletedEvent : IEvent
    {
        public DataObjectDeletedEvent(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public long DataObjectId { get; }

        private sealed class EqualityComparer : IEqualityComparer<DataObjectDeletedEvent>
        {
            public bool Equals(DataObjectDeletedEvent x, DataObjectDeletedEvent y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.DataObjectType == y.DataObjectType && x.DataObjectId == y.DataObjectId;
            }

            public int GetHashCode(DataObjectDeletedEvent obj)
            {
                unchecked
                {
                    return (obj.DataObjectType.GetHashCode() * 397) ^ obj.DataObjectId.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<DataObjectDeletedEvent> Comparer { get; } = new EqualityComparer();
    }
}