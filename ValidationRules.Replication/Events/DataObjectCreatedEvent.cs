using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class DataObjectCreatedEvent : IEvent
    {
        public DataObjectCreatedEvent(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public long DataObjectId { get; }

        private sealed class EqualityComparer : IEqualityComparer<DataObjectCreatedEvent>
        {
            public bool Equals(DataObjectCreatedEvent x, DataObjectCreatedEvent y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.DataObjectType == y.DataObjectType && x.DataObjectId == y.DataObjectId;
            }

            public int GetHashCode(DataObjectCreatedEvent obj)
            {
                unchecked
                {
                    return (obj.DataObjectType.GetHashCode() * 397) ^ obj.DataObjectId.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<DataObjectCreatedEvent> Comparer { get; } = new EqualityComparer();
    }
}