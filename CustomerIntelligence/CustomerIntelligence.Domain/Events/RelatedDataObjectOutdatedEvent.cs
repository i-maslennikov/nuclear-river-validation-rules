using System;

using NuClear.River.Common.Metadata;

namespace NuClear.CustomerIntelligence.Domain.Events
{
    public class RelatedDataObjectOutdatedEvent<TDataObjectId> : IEvent
    {
        public RelatedDataObjectOutdatedEvent(Type relatedDataObjectType, TDataObjectId relatedDataObjectId)
        {
            RelatedDataObjectType = relatedDataObjectType;
            RelatedDataObjectId = relatedDataObjectId;
        }

        public Type RelatedDataObjectType { get; }
        public TDataObjectId RelatedDataObjectId { get; set; }
    }
}