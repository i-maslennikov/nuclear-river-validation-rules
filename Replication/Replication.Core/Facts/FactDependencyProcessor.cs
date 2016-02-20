using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.AdvancedSearch.Common.Metadata.Features;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Replication.Core.API.Facts;
using NuClear.Storage.API.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Facts
{
    public class FactDependencyProcessor<TFact> : IFactDependencyProcessor
    {
        private readonly IQuery _query;
        private readonly IFactDependencyFeature<TFact, long> _metadata;

        public FactDependencyProcessor(IFactDependencyFeature<TFact, long> metadata, IQuery query)
        {
            _query = query;
            _metadata = metadata;
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
                var filter = _metadata.FindSpecificationProvider.Invoke(factIds);
                return operationFactory.Invoke(filter).Map(_query).ToArray();
            }
        }
    }
}