using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Primary
{
    // todo: является полной копией ImportFactsFromErmAccumulator из CI
    public sealed class ImportFactsFromErmAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromErmFlow, TrackedUseCase, OperationAggregatableMessage<FactOperation>>
    {
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly IEntityTypeMappingRegistry<FactsSubDomain> _registry;
        private readonly TrackedUseCaseFiltrator<FactsSubDomain> _useCaseFiltrator;

        public ImportFactsFromErmAccumulator(ITracer tracer, ITelemetryPublisher telemetryPublisher, IEntityTypeMappingRegistry<FactsSubDomain> registry, TrackedUseCaseFiltrator<FactsSubDomain> useCaseFiltrator)
        {
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _registry = registry;
            _useCaseFiltrator = useCaseFiltrator;
        }

        protected override OperationAggregatableMessage<FactOperation> Process(TrackedUseCase message)
        {
            _tracer.DebugFormat("Processing TUC {0}", message.Id);

            var receivedOperationCount = message.Operations.Sum(x => x.AffectedEntities.Changes.Sum(y => y.Value.Sum(z => z.Value.Count)));
            _telemetryPublisher.Publish<ErmReceivedOperationCountIdentity>(receivedOperationCount);

            var changes = _useCaseFiltrator.Filter(message);

            // TODO: вместо кучи factoperation можно передавать одну с dictionary, где уже всё сгруппировано по entity type 
            var factOperations = changes.SelectMany(x => x.Value.Select(y => new FactOperation(_registry.GetEntityType(x.Key), y))).ToList();
            _telemetryPublisher.Publish<ErmEnqueuedOperationCountIdentity>(factOperations.Count);

            return new OperationAggregatableMessage<FactOperation>
            {
                TargetFlow = MessageFlow,
                Operations = factOperations,
                OperationTime = message.Context.Finished.UtcDateTime,
            };
        }
    }
}