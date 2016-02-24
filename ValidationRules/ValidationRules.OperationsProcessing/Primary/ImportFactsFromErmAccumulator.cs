using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.Changes;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Primary
{
    /// <summary>
    /// Стратегия выполняет фильтрацию операций, приехавших в TUC, и преобразование этих операций в операции над фактами.
    /// </summary>
    public sealed class ImportFactsFromErmAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromErmFlow, TrackedUseCase, OperationAggregatableMessage<FactOperation>>
    {
        private readonly IEntityTypeMappingRegistry<FactsSubDomain> _registry;

        public ImportFactsFromErmAccumulator(IEntityTypeMappingRegistry<FactsSubDomain> registry)
        {
            _registry = registry;
        }

        protected override OperationAggregatableMessage<FactOperation> Process(TrackedUseCase message)
        {
            var filteredOperations = Filter(message);
            var factOperations = Convert(filteredOperations).ToArray();

            return new OperationAggregatableMessage<FactOperation>
            {
                TargetFlow = MessageFlow,
                Operations = factOperations,
                OperationTime = message.Context.Finished.UtcDateTime,
            };
        }

        private static IEnumerable<OperationDescriptor> Filter(TrackedUseCase message)
        {
            var operations = (IEnumerable<OperationDescriptor>)message.Operations;

            return operations;
        }

        private IEnumerable<FactOperation> Convert(IEnumerable<OperationDescriptor> operations)
        {
            var factOperations = operations
                .SelectMany(x => ParseEntityType(x.AffectedEntities))
                .GroupBy(x => x.Item1, x => x.Item2)
                .SelectMany(x => x.SelectMany(y => y).Distinct().Select(y => new FactOperation(x.Key, y)));

            return factOperations;
        }

        private IEnumerable<Tuple<Type, IEnumerable<long>>> ParseEntityType(EntityChangesContext changesContext)
        {
            foreach (var change in changesContext.Changes)
            {
                Type entityType;
                var parsed = _registry.TryGetEntityType(change.Key, out entityType);
                if (parsed)
                {
                    yield return Tuple.Create(entityType, (IEnumerable<long>)change.Value.Keys);
                }
            }
        }
    }
}
