using System;

using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.Domain.Events
{
    public class DataObjectReplacedEvent : IDataObjectEvent<StatisticsKey>
    {
        public DataObjectReplacedEvent(Type dataObjectType, StatisticsKey dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public StatisticsKey DataObjectId { get; }
    }
}