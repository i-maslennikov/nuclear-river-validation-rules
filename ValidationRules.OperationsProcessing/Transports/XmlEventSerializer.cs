using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Messages;

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
        private const string RuleCode = "rule";
        private const string OrderId = "orderId";

        private const string PeriodKey = "periodKey";
        private const string Date = "date";

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
                var dataObjectType = @event.Element(DataObjectType);
                var relatedDataObjectType = @event.Element(RelatedDataObjectType);
                var relatedDataObjectId = @event.Element(RelatedDataObjectId);
                if (dataObjectType != null && relatedDataObjectType != null && relatedDataObjectId != null)
                {
                    return new RelatedDataObjectOutdatedEvent<long>(ResolveDataObjectType(dataObjectType.Value), ResolveDataObjectType(relatedDataObjectType.Value), (long)relatedDataObjectId);
                }
            }

            if (IsEventOfType(@event, typeof(RelatedDataObjectOutdatedEvent<PeriodKey>)))
            {
                var dataObjectType = @event.Element(DataObjectType);
                var relatedDataObjectType = @event.Element(RelatedDataObjectType);
                var relatedDataObjectId = @event.Element(RelatedDataObjectId);
                if (dataObjectType != null && relatedDataObjectType != null && relatedDataObjectId != null)
                {
                    var statisticsKey = relatedDataObjectId.Element(PeriodKey);
                    if (statisticsKey != null)
                    {
                        return new RelatedDataObjectOutdatedEvent<PeriodKey>(
                            ResolveDataObjectType(dataObjectType.Value),
                            ResolveDataObjectType(relatedDataObjectType.Value),
                            new PeriodKey
                                {
                                    Date = (DateTime)statisticsKey.Attribute(Date),
                                });
                    }
                }
            }

            if (IsEventOfType(@event, typeof(AmsStateIncrementedEvent)))
            {
                var amsState = @event.Elements(State).Single();
                return new AmsStateIncrementedEvent(new AmsState((long)amsState, (DateTime)amsState.Attribute(Date)));
            }

            if (IsEventOfType(@event, typeof(ErmStateIncrementedEvent)))
            {
                var ermStates = @event.Elements(State);
                return new ErmStateIncrementedEvent(ermStates.Select(x => new ErmState((Guid)x, (DateTime)x.Attribute(Date))));
            }

            if (IsEventOfType(@event, typeof(DelayLoggedEvent)))
            {
                var time = @event.Element(EventHappendTime);
                return new DelayLoggedEvent((DateTime)time);
            }

            if (IsEventOfType(@event, typeof(ResultPartiallyOutdatedEvent)))
            {
                var rule = @event.Element(RuleCode);
                var orderIds = @event.Elements(OrderId);
                return new ResultPartiallyOutdatedEvent((MessageTypeCode)(int)rule, orderIds.Select(x => (long)x).ToList());
            }

            if (IsEventOfType(@event, typeof(ResultOutdatedEvent)))
            {
                var rule = @event.Element(RuleCode);
                return new ResultOutdatedEvent((MessageTypeCode)(int)rule);
            }

            throw new ArgumentException($"Event is unknown or cannot be deserialized: {@event}", nameof(@event));
        }

        public XElement Serialize(IEvent @event)
        {
            switch (@event)
            {
                case FlowEvent flowEvent:
                    return Serialize(flowEvent.Event);

                case DataObjectCreatedEvent createdEvent:
                    return CreateRecord(createdEvent,
                                        new XElement(DataObjectType, createdEvent.DataObjectType.FullName),
                                        new XElement(DataObjectId, createdEvent.DataObjectId));

                case DataObjectUpdatedEvent updatedEvent:
                    return CreateRecord(updatedEvent,
                                        new XElement(DataObjectType, updatedEvent.DataObjectType.FullName),
                                        new XElement(DataObjectId, updatedEvent.DataObjectId));

                case DataObjectDeletedEvent deletedEvent:
                    return CreateRecord(deletedEvent,
                                        new XElement(DataObjectType, deletedEvent.DataObjectType.FullName),
                                        new XElement(DataObjectId, deletedEvent.DataObjectId));

                case RelatedDataObjectOutdatedEvent<long> outdatedEvent:
                    return CreateRecord(outdatedEvent,
                                        new XElement(DataObjectType, outdatedEvent.DataObjectType.FullName),
                                        new XElement(RelatedDataObjectType, outdatedEvent.RelatedDataObjectType.FullName),
                                        new XElement(RelatedDataObjectId, outdatedEvent.RelatedDataObjectId));

                case RelatedDataObjectOutdatedEvent<PeriodKey> complexOutdatedEvent:
                    return CreateRecord(complexOutdatedEvent,
                                        new XElement(DataObjectType, complexOutdatedEvent.DataObjectType.FullName),
                                        new XElement(RelatedDataObjectType, complexOutdatedEvent.RelatedDataObjectType.FullName),
                                        new XElement(RelatedDataObjectId,
                                                     new XElement(PeriodKey,
                                                                  new XAttribute(Date, complexOutdatedEvent.RelatedDataObjectId.Date))));

                case AmsStateIncrementedEvent amsStateIncrementedEvent:
                    return CreateRecord(amsStateIncrementedEvent,
                                        new XElement(State, new XAttribute(Date, amsStateIncrementedEvent.State.UtcDateTime), amsStateIncrementedEvent.State.Offset));

                case ErmStateIncrementedEvent ermStateIncrementedEvent:
                    return CreateRecord(ermStateIncrementedEvent,
                                        ermStateIncrementedEvent.States.Select(x => new XElement(State,
                                                                                                 new XAttribute(Date, x.UtcDateTime), x.Token)).ToArray());

                case DelayLoggedEvent delayLoggedEvent:
                    return CreateRecord(delayLoggedEvent, new XElement(EventHappendTime, delayLoggedEvent.EventTime));

                case ResultOutdatedEvent resultOutdatedEvent:
                    return CreateRecord(resultOutdatedEvent, new XElement(RuleCode, (int)resultOutdatedEvent.Rule));

                case ResultPartiallyOutdatedEvent resultPartiallyOutdatedEvent:
                    {
                        var orderIds = resultPartiallyOutdatedEvent.OrderIds.Select(x => new XElement(OrderId, x));
                        return CreateRecord(resultPartiallyOutdatedEvent, new[] { new XElement(RuleCode, (int)resultPartiallyOutdatedEvent.Rule) }.Concat(orderIds).ToArray());
                    }

                default:
                    throw new ArgumentException($"Unknown event type: {@event.GetType().Name}", nameof(@event));
            }
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