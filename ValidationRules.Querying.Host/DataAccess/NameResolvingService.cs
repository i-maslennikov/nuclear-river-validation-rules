using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Composition;
using NuClear.ValidationRules.Storage.Model.Facts;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.DataAccess
{
    public class NameResolvingService
    {
        private readonly DataConnectionFactory _factory;

        public NameResolvingService(DataConnectionFactory factory)
        {
            _factory = factory;
        }

        public ResolvedNameContainer Resolve(IReadOnlyCollection<Message> messages)
        {
            var references = messages
                .SelectMany(x => x.References)
                .Concat(messages.SelectMany(x => x.References).SelectMany(x => x.Children))
                .Distinct(Reference.Comparer);

            return new ResolvedNameContainer(Resolve(references));
        }

        private IReadOnlyDictionary<Reference, string> Resolve(IEnumerable<Reference> references)
        {
            var searchKeys = references.Select(x => new { x.Id, x.EntityType });

            using (var connection = _factory.CreateDataConnection("Facts"))
            {
                return connection
                    .GetTable<EntityName>()
                    .Where(x => searchKeys.Contains(new { x.Id, x.EntityType }))
                    .ToDictionary(x => new Reference(x.EntityType, x.Id), x => x.Name);
            }
        }
    }
}