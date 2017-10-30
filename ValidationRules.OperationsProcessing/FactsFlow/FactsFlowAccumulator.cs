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
        private readonly ICommandFactory<TrackedUseCase> _commandFactory;

        public FactsFlowAccumulator(IQuery query, ITracer tracer)
        {
            _query = query;
            _tracer = tracer;
            _commandFactory = new FactsCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase trackedUseCase)
        {
            WaitForTucToBeCommitted(trackedUseCase.Id);

            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = _commandFactory.CreateCommands(trackedUseCase),
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

            _tracer.Warn($"Ignored TUC {id} after {TotalWaitMilliseconds}ms waiting");
        }

        private sealed class FactsCommandFactory : ICommandFactory<TrackedUseCase>
        {
            public IReadOnlyCollection<ICommand> CreateCommands(TrackedUseCase message)
            {
                var changes = message.Operations.SelectMany(x => x.AffectedEntities.Changes);
                return changes.SelectMany(x => CommandsForEntityType(x.Key.Id, x.Value.Keys)).Concat(CreateIncrementErmStateCommand(message)).ToList();
            }

            private static IEnumerable<ICommand> CreateIncrementErmStateCommand(TrackedUseCase message)
            {
                return new[] { new IncrementErmStateCommand(new[] { new ErmState(message.Id, message.Context.Finished.UtcDateTime) }) };
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
    }
}