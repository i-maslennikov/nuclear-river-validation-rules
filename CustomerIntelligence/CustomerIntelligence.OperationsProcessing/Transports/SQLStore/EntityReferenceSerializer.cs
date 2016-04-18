using System;
using System.Collections.Generic;
using System.Xml.Linq;

using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports.SQLStore
{
    public sealed class EntityReferenceSerializer : IEntityReferenceSerializer
    {
        private readonly IEntityTypeParser _entityTypeParser;

        public EntityReferenceSerializer(IEntityTypeParser entityTypeParser)
        {
            _entityTypeParser = entityTypeParser;
        }

        public EntityReference Deserialize(XElement element)
        {
            var entityTypeId = (int)element.Attribute("type");
            var entityType = _entityTypeParser.Parse(entityTypeId);
            var key = ReadKey(element.Element("key"));
            return new EntityReference(entityType, key);
        }

        private object ReadKey(XElement element)
        {
            switch (element.Attribute("type").Value)
            {
                case "long":
                    return (long)element.Attribute("id");
                case "StatisticsKey":
                    return new StatisticsKey { ProjectId = (long)element.Attribute("projectId"), CategoryId = (long)element.Attribute("categoryId") };
                default:
                    throw new ArgumentException($"Can not deserialize key of type {element.Attribute("type").Value}", nameof(element));
            }
        }

        public XElement Serialize(string name, EntityReference reference)
        {
            return new XElement(name,
                                new XAttribute("type", reference.EntityType.Id),
                                new XElement("key", WriteEntityKey(reference.EntityKey)));
        }

        private IEnumerable<XAttribute> WriteEntityKey(object entityKey)
        {
            if (entityKey is long)
            {
                return new []
                    {
                        new XAttribute("type", "long"),
                        new XAttribute("id", entityKey),
                    };
            }

            var statisticsKey = entityKey as StatisticsKey;
            if (statisticsKey != null)
            {
                return new[]
                    {
                        new XAttribute("type", "StatisticsKey"),
                        new XAttribute("projectId", statisticsKey.ProjectId),
                        new XAttribute("categoryId", statisticsKey.CategoryId),
                    };
            }

            throw new ArgumentException($"No serialization defined for {entityKey.GetType().Name}");
        }
    }
}