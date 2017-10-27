using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Storage.API.Readings;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.OperationsProcessing.FactsFlow
{
    public sealed class FactsFlowAccumulator : MessageProcessingContextAccumulatorBase<FactsFlow, TrackedUseCase, AggregatableMessage<ICommand>>
    {
        private const int TotalWaitMilliseconds = 60000;

        private readonly IQuery _query;
        private readonly ITracer _tracer;
        private readonly ICommandFactory _commandFactory;

        public FactsFlowAccumulator(IQuery query, ITracer tracer)
        {
            _query = query;
            _tracer = tracer;
            _commandFactory = new FactsCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase trackedUseCase)
        {
            var commands = _commandFactory.CreateCommands(new TrackedUseCaseEvent(trackedUseCase)).ToList();

            var date = trackedUseCase.Context.Finished.UtcDateTime;
            commands.Add(new IncrementErmStateCommand(new[] { new ErmState(trackedUseCase.Id, date) }));

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
            };
        }

        private sealed class FactsCommandFactory : ICommandFactory
        {
            public IEnumerable<ICommand> CreateCommands(IEvent @event)
            {
                var trackedUseCase = ((TrackedUseCaseEvent)@event).TrackedUseCase;
                var changes = trackedUseCase.Operations.SelectMany(x => x.AffectedEntities.Changes);
                return changes.SelectMany(x => CommandsForEntityType(x.Key.Id, x.Value.Keys));
            }

            private static IEnumerable<ICommand> CommandsForEntityType(int entityTypeId, IEnumerable<long> ids)
            {
                var commands = Enumerable.Empty<ICommand>();

                if (EntityTypeMap.TryGetFactTypes(entityTypeId, out var factTypes))
                {
                    var syncDataObjectCommands = from factType in factTypes
                                                 from id in ids
                                                 select new SyncDataObjectCommand(factType, id);

                    commands = commands.Concat(syncDataObjectCommands);
                }

                return commands;
            }
        }

        private sealed class TrackedUseCaseEvent : IEvent
        {
            public TrackedUseCaseEvent(TrackedUseCase trackedUseCase)
            {
                TrackedUseCase = trackedUseCase;
            }

            public TrackedUseCase TrackedUseCase { get; }
        }
    }
}