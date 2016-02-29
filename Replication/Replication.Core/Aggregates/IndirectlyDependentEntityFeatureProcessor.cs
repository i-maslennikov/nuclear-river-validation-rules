using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata.Features;
using NuClear.River.Common.Metadata.Model;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.Core.Aggregates
{
    /// <summary>
    /// Выполняет обработку прямой зависимости между фактом и сущностью.
    /// Приямая зависимость означет, что идентификатор факта является идентиифкатором сущности.
    /// </summary>
    public class IndirectlyDependentEntityFeatureProcessor<TFact, TEntityKey> : IFactDependencyProcessor
        where TFact : class, IIdentifiable<long>
    {
        private readonly IQuery _query;
        private readonly ICommandFactory<TEntityKey> _commandFactory;
        private readonly IndirectlyDependentEntityFeature<TFact, TEntityKey> _metadata;
        private readonly FindSpecificationProvider<TFact, long> _findSpecificationProvider;

        public IndirectlyDependentEntityFeatureProcessor(IndirectlyDependentEntityFeature<TFact, TEntityKey> metadata, IQuery query, IIdentityProvider<long> identityProvider, ICommandFactory<TEntityKey> commandFactory)
        {
            _query = query;
            _metadata = metadata;
            _commandFactory = commandFactory;
            _findSpecificationProvider = new FindSpecificationProvider<TFact, long>(identityProvider);
        }

        public IEnumerable<IOperation> ProcessCreation(IReadOnlyCollection<long> factIds)
        {
            return Process(factIds);
        }

        public IEnumerable<IOperation> ProcessUpdating(IReadOnlyCollection<long> factIds)
        {
            return Process(factIds);
        }

        public IEnumerable<IOperation> ProcessDeletion(IReadOnlyCollection<long> factIds)
        {
            return Process(factIds);
        }

        private IEnumerable<IOperation> Process(IReadOnlyCollection<long> factIds)
        {
            using (Probe.Create("Querying dependent entities"))
            {
                var filter = _findSpecificationProvider.Create(factIds);
                return _metadata.DependentAggregateSpecProvider.Invoke(filter).Map(_query).Select(key => _commandFactory.Create(_metadata.EntityType, key)).ToArray();
            }
        }
    }
}