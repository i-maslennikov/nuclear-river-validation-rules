using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromErmFlow, TrackedUseCase, AggregatableMessage<ICommand>>
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

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase @event)
        {
            _tracer.DebugFormat("Processing TUC {0}", @event.Id);

            var receivedOperationCount = @event.Operations.Sum(x => x.AffectedEntities.Changes.Sum(y => y.Value.Sum(z => z.Value.Count)));
            _telemetryPublisher.Publish<ErmReceivedOperationCountIdentity>(receivedOperationCount);

            var changes = _useCaseFiltrator.Filter(@event);

            var incrementStateCommand = new IncrementStateCommand(new[] { @event.Id });
            var delayCommand = new RecordDelayCommand(@event.Context.Finished.UtcDateTime);

            var commands = changes.SelectMany(x => x.Value.Select(y => (ICommand)new SyncDataObjectCommand(_registry.GetEntityType(x.Key), y))).ToList();
            commands.Add(incrementStateCommand);
            commands.Add(delayCommand);

            _telemetryPublisher.Publish<ErmEnqueuedOperationCountIdentity>(commands.Count);

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
            };
        }
    }
}