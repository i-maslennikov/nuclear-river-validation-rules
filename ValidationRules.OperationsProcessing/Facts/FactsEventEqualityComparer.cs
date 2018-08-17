using System;
using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.Facts
{
    public sealed class FactsEventEqualityComparer : IEqualityComparer<IEvent>
    {
        public bool Equals(IEvent x, IEvent y)
        {
            switch (x)
            {
                case DataObjectCreatedEvent dataObjectCreatedEventX:
                    return y is DataObjectCreatedEvent dataObjectCreatedEventY && DataObjectCreatedEvent.Comparer.Equals(dataObjectCreatedEventX, dataObjectCreatedEventY);
                case DataObjectDeletedEvent dataObjectDeletedEventX:
                    return y is DataObjectDeletedEvent dataObjectDeletedEventY && DataObjectDeletedEvent.Comparer.Equals(dataObjectDeletedEventX, dataObjectDeletedEventY);
                case DataObjectUpdatedEvent dataObjectUpdatedEventX:
                    return y is DataObjectUpdatedEvent dataObjectUpdatedEventY && DataObjectUpdatedEvent.Comparer.Equals(dataObjectUpdatedEventX, dataObjectUpdatedEventY);
                case RelatedDataObjectOutdatedEvent<long> relatedDataObjectOutdatedEventLongX:
                    return y is RelatedDataObjectOutdatedEvent<long> relatedDataObjectOutdatedEventLongY && RelatedDataObjectOutdatedEvent<long>.Comparer.Equals(relatedDataObjectOutdatedEventLongX, relatedDataObjectOutdatedEventLongY);
                case RelatedDataObjectOutdatedEvent<PeriodKey> relatedDataObjectOutdatedEventPeriodKeyX:
                    return y is RelatedDataObjectOutdatedEvent<PeriodKey> relatedDataObjectOutdatedEventPeriodKeyY && RelatedDataObjectOutdatedEvent<PeriodKey>.Comparer.Equals(relatedDataObjectOutdatedEventPeriodKeyX, relatedDataObjectOutdatedEventPeriodKeyY);
                default:
                    throw new ArgumentOutOfRangeException(nameof(x));
            }
        }

        public int GetHashCode(IEvent obj)
        {
            switch (obj)
            {
                case DataObjectCreatedEvent dataObjectCreatedEvent:
                    return DataObjectCreatedEvent.Comparer.GetHashCode(dataObjectCreatedEvent);
                case DataObjectDeletedEvent dataObjectDeletedEvent:
                    return DataObjectDeletedEvent.Comparer.GetHashCode(dataObjectDeletedEvent);
                case DataObjectUpdatedEvent dataObjectUpdatedEvent:
                    return DataObjectUpdatedEvent.Comparer.GetHashCode(dataObjectUpdatedEvent);
                case RelatedDataObjectOutdatedEvent<long> relatedDataObjectOutdatedEventLong:
                    return RelatedDataObjectOutdatedEvent<long>.Comparer.GetHashCode(relatedDataObjectOutdatedEventLong);
                case RelatedDataObjectOutdatedEvent<PeriodKey> relatedDataObjectOutdatedEventPeriodKey:
                    return RelatedDataObjectOutdatedEvent<PeriodKey>.Comparer.GetHashCode(relatedDataObjectOutdatedEventPeriodKey);
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj));
            }
        }
    }
}