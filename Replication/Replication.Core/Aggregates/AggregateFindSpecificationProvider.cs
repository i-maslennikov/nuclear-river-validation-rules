using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateFindSpecificationProvider<T, TKey> : IFindSpecificationProvider<T, AggregateOperation>
        where T : IIdentifiable<TKey>
    {
        private readonly FindSpecificationProvider<T, TKey> _specificationProvider;

        public AggregateFindSpecificationProvider(IIdentityProvider<TKey> identityProvider)
        {
            _specificationProvider = new FindSpecificationProvider<T, TKey>(identityProvider);
        }

        public FindSpecification<T> Create(IEnumerable<AggregateOperation> commands)
        {
            // todo: хак, TKey должен быть long. А вот если бы была возможность из команды получить TKey... (см. задачу "унификация контекста")
            return _specificationProvider.Create(commands.Select(c => c.AggregateId).Distinct().Cast<TKey>());
        }
    }
}