using System;

namespace NuClear.CustomerIntelligence.Domain.Events
{
    public class DataObjectDeletedEvent : IDataObjectEvent<long>
    {
        public DataObjectDeletedEvent(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public long DataObjectId { get; }
    }
}