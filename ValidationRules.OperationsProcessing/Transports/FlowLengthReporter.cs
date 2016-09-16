using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Telemetry;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Transports
{
    public sealed class FlowLengthReporter : IFlowLengthReporter
    {
        private readonly ITelemetryPublisher _telemetry;
        private readonly IDictionary<Guid, Action<int>> _flowReporters;

        public FlowLengthReporter(ITelemetryPublisher telemetry)
        {
            _telemetry = telemetry;
            _flowReporters = new Dictionary<Guid, Action<int>>
                {
                    { CommonEventsFlow.Instance.Id, length => _telemetry.Publish<FinalProcessingAggregateQueueLengthIdentity>(length) },
                    { ImportFactsFromErmFlow.Instance.Id, length => _telemetry.Publish<PrimaryProcessingQueueLengthIdentity>(length) },
                };
        }

        public IReadOnlyCollection<IMessageFlow> SeriviceBusFlows
            => new[] { ImportFactsFromErmFlow.Instance };

        public IReadOnlyCollection<IMessageFlow> SqlFlows
            => new[] { CommonEventsFlow.Instance };

        public void ReportFlowLength(IMessageFlow flow, int length)
        {
            Action<int> reporter;
            if (!_flowReporters.TryGetValue(flow.Id, out reporter))
            {
                throw new ArgumentException($"Поток не поддеривается: {flow.GetType().Name}", nameof(flow));
            }

            reporter.Invoke(length);
        }
    }
}