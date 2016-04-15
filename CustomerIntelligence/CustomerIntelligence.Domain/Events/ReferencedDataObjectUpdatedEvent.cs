using System;

namespace NuClear.CustomerIntelligence.Domain.Events
{
    public class ReferencedDataObjectUpdatedEvent<TKey> : IDataObjectEvent<TKey>
    {
        public ReferencedDataObjectUpdatedEvent(Type dataObjectType, TKey dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public TKey DataObjectId { get; }
    }
}