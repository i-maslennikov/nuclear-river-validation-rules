using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Primary;
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
        private readonly TrackedUseCaseFiltrator<FactsSubDomain> _useCaseFiltrator;

        public ImportFactsFromErmAccumulator(IEntityTypeMappingRegistry<FactsSubDomain> registry, TrackedUseCaseFiltrator<FactsSubDomain> useCaseFiltrator)
        {
            _registry = registry;
            _useCaseFiltrator = useCaseFiltrator;
        }

        protected override OperationAggregatableMessage<FactOperation> Process(TrackedUseCase message)
        {
            var changes = _useCaseFiltrator.Filter(message);

            // TODO: вместо кучи factoperation можно передавать одну с dictionary, где уже всё сгруппировано по entity type 
            var factOperations = changes.SelectMany(x => x.Value.Select(y => new FactOperation(_registry.GetEntityType(x.Key), y))).ToList();

            return new OperationAggregatableMessage<FactOperation>
            {
                TargetFlow = MessageFlow,
                Operations = factOperations,
                OperationTime = message.Context.Finished.UtcDateTime,
            };
        }
    }
}