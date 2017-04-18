using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.OperationsProcessing.FactsFlow
{
    public sealed class FactsFlowAccumulator : MessageProcessingContextAccumulatorBase<FactsFlow, TrackedUseCase, AggregatableMessage<ICommand>>
    {
        private readonly ICommandFactory _commandFactory;

        public FactsFlowAccumulator()
        {
            _commandFactory = new FactsFlowCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase trackedUseCase)
        {
            var commands = _commandFactory.CreateCommands(new TrackedUseCaseEvent(trackedUseCase)).ToList();

            commands.Add(new IncrementStateCommand(new[] { trackedUseCase.Id }));
            commands.Add(new LogDelayCommand(trackedUseCase.Context.Finished.UtcDateTime));

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
            };
        }

        private sealed class FactsFlowCommandFactory : ICommandFactory
        {
            public IEnumerable<ICommand> CreateCommands(IEvent @event)
            {
                var importFactsFromErmEvent = @event as TrackedUseCaseEvent;
                if (importFactsFromErmEvent != null)
                {
                    var changes = importFactsFromErmEvent.TrackedUseCase.Operations.SelectMany(x => x.AffectedEntities.Changes);
                    return changes.SelectMany(x => CommandsForEntityType(x.Key.Id, x.Value.Keys));
                }

                throw new ArgumentException($"Unexpected event '{@event}'", nameof(@event));
            }

            private static IEnumerable<ICommand> CommandsForEntityType(int entityTypeId, IEnumerable<long> ids)
            {
                var commands = Enumerable.Empty<ICommand>();

                IReadOnlyCollection<Type> factTypes;
                if (EntityTypeMap.TryGetFactTypes(entityTypeId, out factTypes))
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