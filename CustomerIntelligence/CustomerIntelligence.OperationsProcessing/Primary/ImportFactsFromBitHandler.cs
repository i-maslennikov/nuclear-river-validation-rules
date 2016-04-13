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
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitHandler : IMessageProcessingHandler
    {
        private readonly IReplaceDataObjectsActorFactory _replaceDataObjectsActorFactory;
        private readonly IOperationSender<RecalculateStatisticsOperation> _sender;
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ImportFactsFromBitHandler(
            IReplaceDataObjectsActorFactory replaceDataObjectsActorFactory,
            IOperationSender<RecalculateStatisticsOperation> sender,
            ITracer tracer,
            ITelemetryPublisher telemetryPublisher)
        {
            _replaceDataObjectsActorFactory = replaceDataObjectsActorFactory;
            _sender = sender;
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
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
                        _sender.Push(events.Cast<RecalculateStatisticsOperation>());
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
    }
}
