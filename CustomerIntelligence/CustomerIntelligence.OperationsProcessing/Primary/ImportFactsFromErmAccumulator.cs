using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Contexts;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    /// <summary>
    /// Applies filter for TUC and maps them to SyncFactCommand
    /// </summary>
    public sealed class ImportFactsFromErmAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromErmFlow, TrackedUseCase, AggregatableMessage<ICommand>>
    {
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly IEntityTypeMappingRegistry<FactsSubDomain> _registry;
        private readonly TrackedUseCaseFiltrator<FactsSubDomain> _useCaseFiltrator;

        public ImportFactsFromErmAccumulator(
            ITracer tracer,
            ITelemetryPublisher telemetryPublisher,
            IEntityTypeMappingRegistry<FactsSubDomain> registry,
            TrackedUseCaseFiltrator<FactsSubDomain> useCaseFiltrator)
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

            var commands = changes.SelectMany(x => x.Value.Select(y => new SyncDataObjectCommand(_registry.GetEntityType(x.Key), y))).ToArray();

            var delayCommand = new RecordDelayCommand(@event.Context.Finished.UtcDateTime);

            _telemetryPublisher.Publish<ErmEnqueuedOperationCountIdentity>(commands.Length);

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = new ICommand[] { delayCommand }.Concat(commands).ToArray(),
            };
        }
    }
}