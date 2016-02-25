using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Facts
{
    public class FactDependencyProcessor<TFact> : IFactDependencyProcessor
        where TFact : IIdentifiable<long>
    {
        private readonly IQuery _query;
        private readonly IFactDependencyFeature<TFact> _metadata;
        private readonly Func<IEnumerable<long>, FindSpecification<TFact>> _filterProvider;

        public FactDependencyProcessor(IIdentityProvider<long> factIdentityProvider, IFactDependencyFeature<TFact> metadata, IQuery query)
        {
            _query = query;
            _metadata = metadata;
            _filterProvider = ids => new FindSpecification<TFact>(factIdentityProvider.Create<TFact, long>(ids));
        }

        public IEnumerable<IOperation> ProcessCreation(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, _metadata.MapSpecificationProviderOnCreate);
        }

        public IEnumerable<IOperation> ProcessUpdating(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, _metadata.MapSpecificationProviderOnUpdate);
        }

        public IEnumerable<IOperation> ProcessDeletion(IReadOnlyCollection<long> factIds)
        {
            return ProcessDependencies(factIds, _metadata.MapSpecificationProviderOnDelete);
        }

        private IEnumerable<IOperation> ProcessDependencies(IReadOnlyCollection<long> factIds, MapToObjectsSpecProvider<TFact, IOperation> operationFactory)
        {
            using (Probe.Create("Querying dependent aggregates"))
            {
                var filter = _filterProvider.Invoke(factIds);
                return operationFactory.Invoke(filter).Map(_query).ToArray();
            }
        }
    }
}