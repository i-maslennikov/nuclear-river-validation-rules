using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.OperationsLogging.API;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.OperationsProcessing;
using NuClear.Tracing.API;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow
{
    public sealed class RulesetFactsFlowHandler : IMessageProcessingHandler
    {
        private readonly IDataObjectsActorFactory _dataObjectsActorFactory;
        private readonly IEventLogger _eventLogger;
        private readonly ITracer _tracer;

        private readonly TransactionOptions _transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.Zero
            };

        public RulesetFactsFlowHandler(
            IDataObjectsActorFactory dataObjectsActorFactory,
            IEventLogger eventLogger,
            ITracer tracer)
        {
            _dataObjectsActorFactory = dataObjectsActorFactory;
            _eventLogger = eventLogger;
            _tracer = tracer;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions))
                {
                    var commands = processingResultsMap.SelectMany(x => x.Value)
                                                       .Cast<AggregatableMessage<ICommand>>()
                                                       .SelectMany(x => x.Commands)
                                                       .ToList();
                    var events = Handle(commands.OfType<IReplaceDataObjectCommand>().ToList());
                    var replaceEvents = events.Select(x => new FlowEvent(RulesetFactsFlow.Instance, x))
                                              .ToList();

                    using (new TransactionScope(TransactionScopeOption.Suppress))
                    {
                        _eventLogger.Log<IEvent>(replaceEvents);
                    }

                    transaction.Complete();
                }

                return processingResultsMap.Keys
                                           .Select(bucketId => MessageProcessingStage.Handling
                                                                                          .ResultFor(bucketId)
                                                                                          .AsSucceeded());
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import facts for AMS");
                return processingResultsMap.Keys
                                           .Select(bucketId => MessageProcessingStage.Handling
                                                                                          .ResultFor(bucketId)
                                                                                          .AsFailed()
                                                                                          .WithExceptions(ex));
            }
        }

        private IEnumerable<IEvent> Handle(IReadOnlyCollection<IReplaceDataObjectCommand> commands)
        {
            if (!commands.Any())
            {
                return Array.Empty<IEvent>();
            }

            var actors = _dataObjectsActorFactory.Create();
            var events = new HashSet<IEvent>();

            foreach (var actor in actors)
            {
                events.UnionWith(actor.ExecuteCommands(commands));
            }

            return events;
        }
    }
}
