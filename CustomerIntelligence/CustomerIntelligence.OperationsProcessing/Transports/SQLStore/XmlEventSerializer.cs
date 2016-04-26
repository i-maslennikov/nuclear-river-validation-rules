using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Replication;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore
{
    public sealed class XmlEventSerializer : IEventSerializer
    {
        private const string EventType = "type";
        private const string DataObjectType = "dataObjectType";
        private const string DataObjectId = "dataObjectId";
        private const string RelatedDataObjectType = "relatedDataObjectType";
        private const string RelatedDataObjectId = "relatedDataObjectId";
        private const string StatisticsKey = "statisticsKey";
        private const string ProjectId = "projectId";
        private const string CategoryId = "categoryId";

        private static readonly IReadOnlyDictionary<string, Type> SimpleTypes =
            AppDomain.CurrentDomain.GetAssemblies()
                     .Where(x => x.FullName.Contains("CustomerIntelligence"))
                     .SelectMany(x => x.ExportedTypes)
                     .Where(x => x.IsClass && !x.IsAbstract && !x.IsGenericType)
                     .ToDictionary(x => x.FullName, x => x);

        public IEvent Deserialize(PerformedOperationFinalProcessing message)
        {
            var @event = XElement.Parse(message.Context);
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

            if (IsEventOfType(@event, typeof(DataObjectReplacedEvent)))
            {
                var dataObjectType = @event.Element(DataObjectType);
                var dataObjectId = @event.Element(DataObjectId);
                if (dataObjectType != null && dataObjectId != null)
                {
                    return new DataObjectReplacedEvent(ResolveDataObjectType(dataObjectType.Value), (long)dataObjectId);
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

            if (IsEventOfType(@event, typeof(RelatedDataObjectOutdatedEvent<StatisticsKey>)))
            {
                var relatedDataObjectType = @event.Element(RelatedDataObjectType);
                var relatedDataObjectId = @event.Element(RelatedDataObjectId);
                if (relatedDataObjectType != null && relatedDataObjectId != null)
                {
                    var statisticsKey = relatedDataObjectId.Element(StatisticsKey);
                    if (statisticsKey != null)
                    {
                        return new RelatedDataObjectOutdatedEvent<StatisticsKey>(
                            ResolveDataObjectType(relatedDataObjectType.Value),
                            new StatisticsKey { ProjectId = (long)statisticsKey.Attribute(ProjectId), CategoryId = (long)statisticsKey.Attribute(CategoryId) });
                    }
                }
            }

            throw new ArgumentException($"Event is unknown or cannot be deserialized: {@event}", nameof(@event));
        }

        public PerformedOperationFinalProcessing Serialize(IEvent @event)
        {
            var createdEvent = @event as DataObjectCreatedEvent;
            if (createdEvent != null)
            {
                return CreateRecord(createdEvent,
                                    new[]
                                        {
                                            new XElement(DataObjectType, createdEvent.DataObjectType.FullName),
                                            new XElement(DataObjectId, createdEvent.DataObjectId)
                                        });
            }

            var updatedEvent = @event as DataObjectUpdatedEvent;
            if (updatedEvent != null)
            {
                return CreateRecord(updatedEvent,
                                    new[]
                                        {
                                            new XElement(DataObjectType, updatedEvent.DataObjectType.FullName),
                                            new XElement(DataObjectId, updatedEvent.DataObjectId)
                                        });
            }

            var deletedEvent = @event as DataObjectDeletedEvent;
            if (deletedEvent != null)
            {
                return CreateRecord(deletedEvent,
                                    new[]
                                        {
                                            new XElement(DataObjectType, deletedEvent.DataObjectType.FullName),
                                            new XElement(DataObjectId, deletedEvent.DataObjectId)
                                        });
            }

            var replacedEvent = @event as DataObjectReplacedEvent;
            if (replacedEvent != null)
            {
                return CreateRecord(replacedEvent,
                                    new[]
                                        {
                                            new XElement(DataObjectType, replacedEvent.DataObjectType.FullName),
                                            new XElement(DataObjectId, replacedEvent.DataObjectId)
                                        });
            }

            var outdatedEvent = @event as RelatedDataObjectOutdatedEvent<long>;
            if (outdatedEvent != null)
            {
                return CreateRecord(outdatedEvent,
                                    new[]
                                        {
                                            new XElement(RelatedDataObjectType, outdatedEvent.RelatedDataObjectType.FullName),
                                            new XElement(RelatedDataObjectId, outdatedEvent.RelatedDataObjectId)
                                        });
            }

            var complexOutdatedEvent = @event as RelatedDataObjectOutdatedEvent<StatisticsKey>;
            if (complexOutdatedEvent != null)
            {
                return CreateRecord(complexOutdatedEvent,
                                    new[]
                                        {
                                            new XElement(RelatedDataObjectType, complexOutdatedEvent.RelatedDataObjectType.FullName),
                                            new XElement(RelatedDataObjectId,
                                                         new XElement(StatisticsKey,
                                                                      new XAttribute(ProjectId, complexOutdatedEvent.RelatedDataObjectId.ProjectId),
                                                                      new XAttribute(CategoryId, complexOutdatedEvent.RelatedDataObjectId.CategoryId)))
                                        });
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

        private static PerformedOperationFinalProcessing CreateRecord(IEvent @event, IReadOnlyCollection<XElement> elements)
        {
            return new PerformedOperationFinalProcessing
                {
                    OperationId = Guid.NewGuid(),
                    Context = new XElement("event", new XAttribute(EventType, @event.GetType().GetFriendlyName()), elements).ToString(SaveOptions.DisableFormatting)
                };
        }
    }
}