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

        public FactsFlowAccumulator(IQuery query, ITracer tracer)
        {
            _query = query;
            _tracer = tracer;
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase trackedUseCase)
        {
            WaitForTucToBeCommitted(trackedUseCase.Id);

            var commands = CommandFactory.CreateCommands(trackedUseCase).ToList();

            var date = trackedUseCase.Context.Finished.UtcDateTime;
            commands.Add(new IncrementErmStateCommand(new[] { new ErmState(trackedUseCase.Id, date) }));

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

            _tracer.Warn($"Ignored TUC {id} after {TotalWaitMilliseconds}ms waiting");
        }

        private static class CommandFactory
        {
            public static IEnumerable<ICommand> CreateCommands(TrackedUseCase trackedUseCase)
            {
                var changes = trackedUseCase.Operations.SelectMany(x => x.AffectedEntities.Changes);
                return changes.SelectMany(x => CommandsForEntityType(x.Key.Id, x.Value.Keys));
            }

            private static IEnumerable<ICommand> CommandsForEntityType(int entityTypeId, IEnumerable<long> ids)
            {
                var commands = Enumerable.Empty<ICommand>();

                if (EntityTypeMap.TryGetFactTypes(entityTypeId, out IReadOnlyCollection<Type> factTypes))
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