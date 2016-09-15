using System.Collections.Generic;
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
        private readonly CommandFactory<AccountFactsSubDomain> _accountCommandFactory;
        private readonly CommandFactory<ConsistencyFactsSubDomain> _consistencyCommandFactory;
        private readonly CommandFactory<PriceFactsSubDomain> _priceCommandFactory;
        private readonly CommandFactory<AdvertisementFactsSubDomain> _advertisementCommandFactory;

        public ImportFactsFromErmAccumulator(ITracer tracer,
                                             ITelemetryPublisher telemetryPublisher,
                                             CommandFactory<AccountFactsSubDomain> accountCommandFactory,
                                             CommandFactory<ConsistencyFactsSubDomain> consistencyCommandFactory,
                                             CommandFactory<PriceFactsSubDomain> priceCommandFactory,
                                             CommandFactory<AdvertisementFactsSubDomain> advertisementCommandFactory)
        {
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _accountCommandFactory = accountCommandFactory;
            _consistencyCommandFactory = consistencyCommandFactory;
            _priceCommandFactory = priceCommandFactory;
            _advertisementCommandFactory = advertisementCommandFactory;
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase @event)
        {
            _tracer.DebugFormat("Processing TUC {0}", @event.Id);

            var receivedOperationCount = @event.Operations.Sum(x => x.AffectedEntities.Changes.Sum(y => y.Value.Sum(z => z.Value.Count)));
            _telemetryPublisher.Publish<ErmReceivedOperationCountIdentity>(receivedOperationCount);


            var incrementStateCommand = new IncrementStateCommand(new[] { @event.Id });
            var delayCommand = new RecordDelayCommand(@event.Context.Finished.UtcDateTime);

            var commands = _accountCommandFactory.CreateCommands(@event)
                .Concat(_consistencyCommandFactory.CreateCommands(@event))
                .Concat(_priceCommandFactory.CreateCommands(@event))
                .Concat(_advertisementCommandFactory.CreateCommands(@event))
                .ToList();

            commands.Add(incrementStateCommand);
            commands.Add(delayCommand);

            _telemetryPublisher.Publish<ErmEnqueuedOperationCountIdentity>(commands.Count);

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
            };
        }

        public class CommandFactory<TSubDomain>
            where TSubDomain : ISubDomain
        {
            private readonly IEntityTypeMappingRegistry<TSubDomain> _registry;
            private readonly TrackedUseCaseFiltrator<TSubDomain> _useCaseFiltrator;

            public CommandFactory(IEntityTypeMappingRegistry<TSubDomain> registry, TrackedUseCaseFiltrator<TSubDomain> useCaseFiltrator)
            {
                _registry = registry;
                _useCaseFiltrator = useCaseFiltrator;
            }

            public IEnumerable<ICommand> CreateCommands(TrackedUseCase @event)
            {
                var changes = _useCaseFiltrator.Filter(@event);
                return changes.SelectMany(x => x.Value.Select(y => (ICommand)new SyncDataObjectCommand(_registry.GetEntityType(x.Key), y)));
            }
        }
    }
}