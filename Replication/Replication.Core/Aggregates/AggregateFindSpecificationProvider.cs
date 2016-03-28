using System.Collections.Generic;
using System.Linq;

using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Aggregates
{
    public sealed class AggregateFindSpecificationProvider<T> : IFindSpecificationProvider<T, long>
        where T : IIdentifiable<long>
    {
        private readonly FindSpecificationProvider<T, long> _specificationProvider;

        public AggregateFindSpecificationProvider(IIdentityProvider<long> identityProvider)
        {
            _specificationProvider = new FindSpecificationProvider<T, long>(identityProvider);
        }

        public FindSpecification<T> Create(IEnumerable<long> ids)
        {
            return _specificationProvider.Create(ids.Distinct());
        }
    }
}