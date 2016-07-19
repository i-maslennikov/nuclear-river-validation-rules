using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.Transports
{
    public sealed class XmlEventSerializer : IXmlEventSerializer
    {
        private const string EventType = "type";
        private const string DataObjectType = "dataObjectType";
        private const string DataObjectId = "dataObjectId";
        private const string RelatedDataObjectType = "relatedDataObjectType";
        private const string RelatedDataObjectId = "relatedDataObjectId";
        private const string State = "state";
        private const string EventHappendTime = "time";

        private const string PeriodKey = "periodKey";
        private const string OrganizationUnitId = "organizationUnitId";
        private const string Start = "start";

        private static readonly IReadOnlyDictionary<string, Type> SimpleTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                     .Where(x => x.FullName.Contains("ValidationRules"))
                     .SelectMany(x => x.ExportedTypes)
                     .Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType)
                     .ToDictionary(x => x.FullName, x => x);

        public IEvent Deserialize(XElement @event)
        {
            if (IsEventOfType(@event, typeof(DataObjectCreatedEvent)))
            {
                var dataObjectType = @event.Element(DataObjectType);
                var dataObjectId = @event.Element(DataObjectId);
                if (dataObjectType != null && dataObjectId != null)
                {
                    return new DataObjectCreatedEvent(ResolveDataObjectType(dataObjectType.Value), (long)dataObjectId);
                }
            }

            if (IsEventOfType(@event, typeof(DataObjectUpdatedEvent)))
            {
                var dataObjectType = @event.Element(DataObjectType);
                var dataObjectId = @event.Element(DataObjectId);
                if (dataObjectType != null && dataObjectId != null)
                {
                    return new DataObjectUpdatedEvent(ResolveDataObjectType(dataObjectType.Value), (long)dataObjectId);
                }
            }

            if (IsEventOfType(@event, typeof(DataObjectDeletedEvent)))
            {
                var dataObjectType = @event.Element(DataObjectType);
                var dataObjectId = @event.Element(DataObjectId);
                if (dataObjectType != null && dataObjectId != null)
                {
                    return new DataObjectDeletedEvent(ResolveDataObjectType(dataObjectType.Value), (long)dataObjectId);
                }
            }

            if (IsEventOfType(@event, typeof(RelatedDataObjectOutdatedEvent<long>)))
            {
                var relatedDataObjectType = @event.Element(RelatedDataObjectType);
                var relatedDataObjectId = @event.Element(RelatedDataObjectId);
                if (relatedDataObjectType != null && relatedDataObjectId != null)
                {
                    return new RelatedDataObjectOutdatedEvent<long>(ResolveDataObjectType(relatedDataObjectType.Value), (long)relatedDataObjectId);
                }
            }

            if (IsEventOfType(@event, typeof(RelatedDataObjectOutdatedEvent<PeriodKey>)))
            {
                var relatedDataObjectType = @event.Element(RelatedDataObjectType);
                var relatedDataObjectId = @event.Element(RelatedDataObjectId);
                if (relatedDataObjectType != null && relatedDataObjectId != null)
                {
                    var statisticsKey = relatedDataObjectId.Element(PeriodKey);
                    if (statisticsKey != null)
                    {
                        return new RelatedDataObjectOutdatedEvent<PeriodKey>(
                            ResolveDataObjectType(relatedDataObjectType.Value),
                            new PeriodKey
                                {
                                    OrganizationUnitId = (long)statisticsKey.Attribute(OrganizationUnitId),
                                    Start = (DateTime)statisticsKey.Attribute(Start)
                                });
                    }
                }
            }

            if (IsEventOfType(@event, typeof(FactsStateIncrementedEvent)))
            {
                var states = @event.Elements(State).Select(x => new Guid(x.Value));
                return new FactsStateIncrementedEvent(states.ToArray());
            }

            if (IsEventOfType(@event, typeof(AggregatesStateIncrementedEvent)))
            {
                var states = @event.Elements(State).Select(x => new Guid(x.Value));
                return new AggregatesStateIncrementedEvent(states.ToArray());
            }

            if (IsEventOfType(@event, typeof(FactsBatchProcessedEvent)))
            {
                var time = @event.Element(EventHappendTime);
                return new FactsBatchProcessedEvent((DateTime)time);
            }

            if (IsEventOfType(@event, typeof(AggregatesBatchProcessedEvent)))
            {
                var time = @event.Element(EventHappendTime);
                return new AggregatesBatchProcessedEvent((DateTime)time);
            }

            throw new ArgumentException($"Event is unknown or cannot be deserialized: {@event}", nameof(@event));
        }

        public XElement Serialize(IEvent @event)
        {
            var createdEvent = @event as DataObjectCreatedEvent;
            if (createdEvent != null)
            {
                return CreateRecord(createdEvent,
                                    new XElement(DataObjectType, createdEvent.DataObjectType.FullName),
                                    new XElement(DataObjectId, createdEvent.DataObjectId));
            }

            var updatedEvent = @event as DataObjectUpdatedEvent;
            if (updatedEvent != null)
            {
                return CreateRecord(updatedEvent,
                                    new XElement(DataObjectType, updatedEvent.DataObjectType.FullName),
                                    new XElement(DataObjectId, updatedEvent.DataObjectId));
            }

            var deletedEvent = @event as DataObjectDeletedEvent;
            if (deletedEvent != null)
            {
                return CreateRecord(deletedEvent,
                                    new XElement(DataObjectType, deletedEvent.DataObjectType.FullName),
                                    new XElement(DataObjectId, deletedEvent.DataObjectId));
            }

            var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
            if (outdatedEvent != null)
            {
                return CreateRecord(outdatedEvent,
                                    new XElement(RelatedDataObjectType, outdatedEvent.RelatedDataObjectType.FullName),
                                    new XElement(RelatedDataObjectId, outdatedEvent.RelatedDataObjectId));
            }

            var complexOutdatedEvent = @event as RelatedDataObjectOutdatedEvent<PeriodKey>;
            if (complexOutdatedEvent != null)
            {
                return CreateRecord(complexOutdatedEvent,
                                    new XElement(RelatedDataObjectType, complexOutdatedEvent.RelatedDataObjectType.FullName),
                                    new XElement(RelatedDataObjectId,
                                                 new XElement(PeriodKey,
                                                              new XAttribute(OrganizationUnitId, complexOutdatedEvent.RelatedDataObjectId.OrganizationUnitId),
                                                              new XAttribute(Start, complexOutdatedEvent.RelatedDataObjectId.Start))));
            }

            var factsStateIncrementedEvent = @event as FactsStateIncrementedEvent;
            if (factsStateIncrementedEvent != null)
            {
                return CreateRecord(factsStateIncrementedEvent,
                                    factsStateIncrementedEvent.IncludedTokens.Select(guid => new XElement(State, guid.ToString())).ToArray());
            }

            var aggregatesStateIncrementedEvent = @event as AggregatesStateIncrementedEvent;
            if (aggregatesStateIncrementedEvent != null)
            {
                return CreateRecord(aggregatesStateIncrementedEvent,
                                    aggregatesStateIncrementedEvent.IncludedTokens.Select(guid => new XElement(State, guid.ToString())).ToArray());
            }

            var factsBatchProcessedEvent = @event as FactsBatchProcessedEvent;
            if (factsBatchProcessedEvent != null)
            {
                return CreateRecord(factsBatchProcessedEvent, new[] { new XElement(EventHappendTime, factsBatchProcessedEvent.EventTime) });
            }

            var aggregateBatchProcessedEvent = @event as AggregatesBatchProcessedEvent;
            if (aggregateBatchProcessedEvent != null)
            {
                return CreateRecord(aggregateBatchProcessedEvent, new[] { new XElement(EventHappendTime, aggregateBatchProcessedEvent.EventTime) });
            }

            throw new ArgumentException($"Unknown event type: {@event.GetType().Name}", nameof(@event));
        }

        private static bool IsEventOfType(XElement @event, Type eventType)
        {
            return @event.Attribute(EventType).Value == eventType.GetFriendlyName();
        }

        private static Type ResolveDataObjectType(string typeName)
        {
            Type type;
            return SimpleTypes.TryGetValue(typeName, out type) ? type : null;
        }

        private static XElement CreateRecord(IEvent @event, params XElement[] elements)
            => new XElement("event", new XAttribute(EventType, @event.GetType().GetFriendlyName()), elements);
    }
}