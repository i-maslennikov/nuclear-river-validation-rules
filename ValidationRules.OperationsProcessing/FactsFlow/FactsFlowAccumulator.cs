using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.OperationsProcessing.FactsFlow
{
    public sealed class FactsFlowAccumulator : MessageProcessingContextAccumulatorBase<FactsFlow, TrackedUseCase, AggregatableMessage<ICommand>>
    {
        private const int TotalWaitMilliseconds = 2000;

        private readonly IQuery _query;
        private readonly ICommandFactory _commandFactory;

        public FactsFlowAccumulator(IQuery query)
        {
            _query = query;
            _commandFactory = new FactsFlowCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase trackedUseCase)
        {
            WaitForTucToBeCommitted(trackedUseCase.Id);

            var commands = _commandFactory.CreateCommands(new TrackedUseCaseEvent(trackedUseCase)).ToList();

            commands.Add(new IncrementStateCommand(new[] { trackedUseCase.Id }));
            commands.Add(new LogDelayCommand(trackedUseCase.Context.Finished.UtcDateTime));

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
            };
        }

        private void WaitForTucToBeCommitted(Guid id)
        {
            for (var i = 0; i < TotalWaitMilliseconds / 100; i++, Task.Delay(100).Wait())
            {
                var transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero };
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                {
                    var completed = _query.For<UseCaseTrackingEvent>().Where(x => x.UseCaseId == id).Any(x => x.EventType == 3 || x.EventType == 4);
                    transaction.Complete();

                    if (completed)
                    {
                        return;
                    }
                }
            }

            throw new Exception($"Looks like TUC '{id}' was not completed by Erm");
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