using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateFindSpecificationProvider<T> : IFindSpecificationProvider<T, AggregateOperation>
        where T : IIdentifiable<long>
    {
        private readonly FindSpecificationProvider<T, long> _specificationProvider;

        public AggregateFindSpecificationProvider(IIdentityProvider<long> identityProvider)
        {
            _specificationProvider = new FindSpecificationProvider<T, long>(identityProvider);
        }

        public FindSpecification<T> Create(IEnumerable<AggregateOperation> commands)
        {
            return _specificationProvider.Create(commands.Select(c => c.EntityId).Distinct());
        }
    }
}