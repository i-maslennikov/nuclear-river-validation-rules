using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Replication.Core.API;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Common.Metadata.Model;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitHandler : IMessageProcessingHandler
    {
        private readonly IReplaceDataObjectsActorFactory _replaceDataObjectsActorFactory;
        private readonly IOperationSender _sender;
        private readonly IOperationDispatcher _operationDispatcher;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromBitHandler(
            IReplaceDataObjectsActorFactory replaceDataObjectsActorFactory,
            IOperationSender sender,
            IOperationDispatcher operationDispatcher,
            ITracer tracer,
            ITelemetryPublisher telemetryPublisher)
        {
            _replaceDataObjectsActorFactory = replaceDataObjectsActorFactory;
            _sender = sender;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
            _operationDispatcher = operationDispatcher;
        }

        public IEnumerable<StageResult> Handle(IReadOnlyDictionary<Guid, List<IAggregatableMessage>> processingResultsMap)
        {
            return processingResultsMap.Select(pair => Handle(pair.Key, pair.Value.OfType<CorporateBusAggregatableMessage>()));
        }

        private StageResult Handle(Guid bucketId, IEnumerable<CorporateBusAggregatableMessage> messages)
        {
            try
            {
                foreach (var message in messages)
                {
                    var commandGroups = message.Commands.GroupBy(x => x.GetType());
                    foreach (var commandGroup in commandGroups)
                    {
                        var replaceFactActor = _replaceDataObjectsActorFactory.Create(commandGroup.Key);

                        var commands = commandGroup.ToArray();
                        var events = replaceFactActor.ExecuteCommands(commands);

                        _telemetryPublisher.Publish<BitStatisticsEntityProcessedCountIdentity>(commands.Length);
                        DispatchOperations(opertaions);
                    }
                }

                return MessageProcessingStage.Handling.ResultFor(bucketId).AsSucceeded();
            }
            catch (Exception ex)
            {
                _tracer.Error(ex, "Error when import facts for BIT");
                return MessageProcessingStage.Handling.ResultFor(bucketId).AsFailed().WithExceptions(ex);
            }
        }

        private void DispatchOperations(IEnumerable<IOperation> opertaions)
        {
            foreach (var pair in _operationDispatcher.Dispatch(opertaions))
            {
                _sender.Push(pair.Value, pair.Key);
            }
        }
    }
}