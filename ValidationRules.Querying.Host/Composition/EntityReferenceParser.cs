using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public sealed class EntityReferenceParser
    {
        private const string ConfigurationString = "Facts";

        private readonly DataConnectionFactory _factory;

        private static readonly Dictionary<string, int> EntityTypeMap = new Dictionary<string, int>
        {
            { nameof(EntityTypeIds.Advertisement), EntityTypeIds.Advertisement },
            { nameof(EntityTypeIds.Category), EntityTypeIds.Category },
            { nameof(EntityTypeIds.Firm), EntityTypeIds.Firm },
            { nameof(EntityTypeIds.FirmAddress), EntityTypeIds.FirmAddress },
            { nameof(EntityTypeIds.LegalPersonProfile), EntityTypeIds.LegalPersonProfile },
            { nameof(EntityTypeIds.Order), EntityTypeIds.Order },
            { nameof(EntityTypeIds.Project), EntityTypeIds.Project },
            { nameof(EntityTypeIds.Theme), EntityTypeIds.Theme },
        };

        public EntityReferenceParser(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public IReadOnlyCollection<MessageWithEntityReferences> ParseEntityReferences(IReadOnlyCollection<Message> messages)
        {
            var messagesWithTuples = messages.Select(message => new
            {
                Message = message,
                Tuples = ParseTuples(message.Xml.Root),
            }).ToList();

            var entityNameMap = GetEntityNameMap(messagesWithTuples.SelectMany(x => x.Tuples.Select(tuple => tuple.Item2)));

            var messagesWithReferences = messagesWithTuples.Select(messageWithTuples =>
            {
                var entityReferences = messageWithTuples.Tuples.Aggregate(new List<EntityReference>(),
                (list, tuple) =>
                {
                    string entityName;
                    if (entityNameMap.TryGetValue(tuple.Item2, out entityName))
                    {
                        tuple.Item1.Name = entityName;
                        list.Add(tuple.Item1);
                    }

                    return list;
                });

                return new MessageWithEntityReferences
                {
                    Message = messageWithTuples.Message,
                    References = entityReferences
                };
            }).ToList();

            return messagesWithReferences;
        }

        private Dictionary<EntityName.EntityNameKey, string> GetEntityNameMap(IEnumerable<EntityName.EntityNameKey> keys)
        {
            var hashSet = new HashSet<EntityName.EntityNameKey>(keys);

            using (var connection = _factory.CreateDataConnection(ConfigurationString))
            {
                var query = connection.GetTable<EntityName>();
                return query
                    .Where(x => hashSet.Contains(new EntityName.EntityNameKey { Id = x.Id, TypeId = x.TypeId }))
                    .ToDictionary(x => new EntityName.EntityNameKey{ Id = x.Id, TypeId = x.TypeId}, x => x.Name);
            }
        }

        private static IReadOnlyCollection<Tuple<EntityReference, EntityName.EntityNameKey>> ParseTuples(XElement xml)
        {
            var tuples = xml.Elements().Aggregate(new List<Tuple<EntityReference, EntityName.EntityNameKey>>(),
            (list, element) =>
            {
                var typeName = element.Name.ToString();

                // skip custom messages
                if (typeName == "message")
                {
                    return list;
                }

                // take 1: parse custom entity references
                Tuple<EntityReference, EntityName.EntityNameKey> tuple;
                if (TryParseCustomEntityReference(element, typeName, out tuple))
                {
                    list.Add(tuple);
                    return list;
                }

                // take 2: parse entity references
                var idAttr = element.Attribute("id");
                if (idAttr != null)
                {
                    var id = (long)idAttr;

                    var entityTypeName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(typeName);
                    int typeId;
                    if (EntityTypeMap.TryGetValue(entityTypeName, out typeId))
                    {
                        tuple = Tuple.Create(
                                    new EntityReference(entityTypeName, id, id.ToString()),
                                    new EntityName.EntityNameKey { Id = id, TypeId = typeId });
                        list.Add(tuple);
                        return list;
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(typeName), typeName, "Unknown xml node name");
            });

            return tuples;
        }

        private static bool TryParseCustomEntityReference(XElement element, string typeName, out Tuple<EntityReference, EntityName.EntityNameKey> tuple)
        {
            switch (typeName)
            {
                case "orderPosition":
                {
                    var id = (long)element.Attribute("id");
                    var positionId = (long)element.Element("position")?.Attribute("id");
                    tuple = Tuple.Create(
                                new EntityReference(nameof(EntityTypeIds.OrderPosition), id, id.ToString()),
                                new EntityName.EntityNameKey { Id = positionId, TypeId = EntityTypeIds.Position });
                    return true;
                }

                case "opa":
                {
                    var orderpositionId = (long)element.Element("orderPosition")?.Attribute("id");
                    var positionId = (long)element.Element("position")?.Attribute("id");
                    tuple = Tuple.Create(
                                new EntityReference(nameof(EntityTypeIds.OrderPosition), orderpositionId, orderpositionId.ToString()),
                                new EntityName.EntityNameKey { Id = positionId, TypeId = EntityTypeIds.Position });
                    return true;
                }

                case "pricePosition":
                {
                    var id = (long)element.Attribute("id");
                    var positionId = (long)element.Element("position")?.Attribute("id");
                    tuple = Tuple.Create(
                                new EntityReference(nameof(EntityTypeIds.PricePosition), id, id.ToString()),
                                new EntityName.EntityNameKey { Id = positionId, TypeId = EntityTypeIds.Position });
                    return true;
                }

                case "advertisementElement":
                {
                    var id = (long)element.Attribute("id");
                    var templateId = (long)element.Element("advertisementElementTemplate")?.Attribute("id");
                    tuple = Tuple.Create(
                                new EntityReference(nameof(EntityTypeIds.AdvertisementElement), id, id.ToString()),
                                new EntityName.EntityNameKey { Id = templateId, TypeId = EntityTypeIds.AdvertisementElementTemplate });
                    return true;
                }

                default:
                {
                    tuple = null;
                    return false;
                }
            }
        }

        public sealed class MessageWithEntityReferences
        {
            public Message Message { get; set; }
            public IReadOnlyCollection<EntityReference> References { get; set; }
        }
    }
}