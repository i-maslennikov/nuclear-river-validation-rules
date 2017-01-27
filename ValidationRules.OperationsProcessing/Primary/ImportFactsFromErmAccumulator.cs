using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromErmAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromErmFlow, TrackedUseCase, AggregatableMessage<ICommand>>
    {
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;
        private readonly ICommandFactory _commandFactory;

        public ImportFactsFromErmAccumulator(ITracer tracer,
                                             ITelemetryPublisher telemetryPublisher)
        {
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _commandFactory = new ImportFactsFromErmCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase trackedUseCase)
        {
            _tracer.DebugFormat("Processing TUC {0}", trackedUseCase.Id);

            _telemetryPublisher.Publish<ErmReceivedOperationCountIdentity>(1);

            var commands = _commandFactory.CreateCommands(new ImportFactsFromErmEvent(trackedUseCase)).ToList();

            commands.Add(new IncrementStateCommand(new[] { trackedUseCase.Id }));
            commands.Add(new LogDelayCommand(trackedUseCase.Context.Finished.UtcDateTime));

            _telemetryPublisher.Publish<ErmEnqueuedOperationCountIdentity>(commands.Count);

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
            };
        }

        private sealed class ImportFactsFromErmCommandFactory : ICommandFactory
        {
            public IEnumerable<ICommand> CreateCommands(IEvent @event)
            {
                var importFactsFromErmEvent = @event as ImportFactsFromErmEvent;
                if (importFactsFromErmEvent == null)
                {
                    return Enumerable.Empty<ICommand>();
                }

                var changes = importFactsFromErmEvent.TrackedUseCase.Operations.SelectMany(x => x.AffectedEntities.Changes);
                return changes.SelectMany(x => SyncDataObjectCommand(x.Key, x.Value.Keys));
            }

            private static IEnumerable<ICommand> SyncDataObjectCommand(IIdentity entityType, IEnumerable<long> ids)
            {
                IReadOnlyCollection<Type> factTypes;
                if (!EntityTypeMap.TryGetFactTypes(entityType.Id, out factTypes))
                {
                    return Enumerable.Empty<ICommand>();
                }

                var commands = from factType in factTypes
                               from id in ids
                               select new SyncDataObjectCommand(factType, id);

                return commands;
            }
        }

        private sealed class ImportFactsFromErmEvent : IEvent
        {
            public TrackedUseCase TrackedUseCase { get; }

            public ImportFactsFromErmEvent(TrackedUseCase trackedUseCase)
            {
                TrackedUseCase = trackedUseCase;
            }
        }
    }
}