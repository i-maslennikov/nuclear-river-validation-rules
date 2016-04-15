using System;

namespace NuClear.CustomerIntelligence.Domain.Events
{
    public class DataObjectCreatedEvent : IDataObjectEvent<long>
    {
        public DataObjectCreatedEvent(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public long DataObjectId { get; }
    }
}