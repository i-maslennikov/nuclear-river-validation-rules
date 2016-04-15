using System;

namespace NuClear.CustomerIntelligence.Domain.Events
{
    public class DataObjectUpdatedEvent : IDataObjectEvent<long>
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