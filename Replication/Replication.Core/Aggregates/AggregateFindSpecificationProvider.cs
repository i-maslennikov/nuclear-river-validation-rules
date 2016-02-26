using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateFindSpecificationProvider<T, TKey> : IFindSpecificationProvider<T>
        where T : IIdentifiable<TKey>
    {
        private readonly IIdentityProvider<TKey> _identityProvider;

        public AggregateFindSpecificationProvider(IIdentityProvider<TKey> identityProvider)
        {
            _identityProvider = identityProvider;
        }

        public FindSpecification<T> Create(IEnumerable<AggregateOperation> commands)
        {
            // todo: вот если бы была возможность из комманды получить TKey... (см. задачу "унификация контекста")
            return new FindSpecification<T>(_identityProvider.Create<T, TKey>(commands.Select(c => c.AggregateId).Distinct().Cast<TKey>()));
        }
    }
}