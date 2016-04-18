using System;

using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.Events
{
    public class DataObjectUpdatedEvent : IEvent
    {
        public DataObjectUpdatedEvent(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public long DataObjectId { get; }
    }
}