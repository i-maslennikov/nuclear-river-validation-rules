using System;
using System.Collections.Generic;
using System.Xml.Linq;

using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.ValidationRules.Domain.Model;

namespace NuClear.ValidationRules.OperationsProcessing.Transports.SQLStore
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
            // todo: похоже, что можно выделить общий класс для всех контекстов и мелких сериализаторов конкретных ключей
            switch (element.Attribute("type").Value)
            {
                case "long":
                    return (long)element.Attribute("id");
                case "PeriodKey":
                    return new PeriodKey
                    {
                        OrganizationUnitId = (long)element.Attribute("organizationUnit"),
                        Start = DateTime.Parse(element.Attribute("start").Value),
                        End = DateTime.Parse(element.Attribute("end").Value)
                    };
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

            var periodKey = entityKey as PeriodKey;
            if (periodKey != null)
            {
                return new[]
                    {
                        new XAttribute("type", "PeriodKey"),
                        new XAttribute("organizationUnit", periodKey.OrganizationUnitId),
                        new XAttribute("start", periodKey.Start.ToString("o")),
                        new XAttribute("end", periodKey.End.ToString("o")),
                    };
            }

            throw new ArgumentException($"No serialization defined for {entityKey.GetType().Name}");
        }
    }
}