using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Storage.Model.Facts;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public class NameResolvingService
    {
        private readonly DataConnectionFactory _factory;
        private readonly IReadOnlyDictionary<int, IEntityType> _knownEntityTypes;

        public NameResolvingService(DataConnectionFactory factory, IReadOnlyCollection<IEntityType> knownEntityTypes)
        {
            _factory = factory;
            _knownEntityTypes = knownEntityTypes.ToDictionary(x => x.Id, x => x);
        }

        public ResolvedNameContainer Resolve(IReadOnlyCollection<Message> messages)
        {
            var references = messages.SelectMany(x => x.References).Concat(messages.SelectMany(x => x.References).SelectMany(x => x.Children)).Distinct(ReferenceComparer.Instance);
            return new ResolvedNameContainer(Resolve(references), _knownEntityTypes);
        }

        private IReadOnlyDictionary<Reference, string> Resolve(IEnumerable<Reference> references)
        {
            var searchKeys = references.Select(x => new { x.Id, x.EntityType });

            using (var connection = _factory.CreateDataConnection("Facts"))
            {
                var query = connection.GetTable<EntityName>();
                return query
                    .Where(x => searchKeys.Contains(new { x.Id, x.EntityType }))
                    .ToDictionary(x => new Reference(x.EntityType, x.Id), x => x.Name);
            }
        }
    }
}