using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Identities.Telemetry;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.Telemetry;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Transports
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
                    { AggregatesFlow.Instance.Id, length => _telemetry.Publish<FinalProcessingAggregateQueueLengthIdentity>(length) },
                    { StatisticsFlow.Instance.Id, length => _telemetry.Publish<FinalProcessingStatisticsQueueLengthIdentity>(length) },
                    { ImportFactsFromErmFlow.Instance.Id, length => _telemetry.Publish<PrimaryProcessingQueueLengthIdentity>(length) },
                };
        }

        public IReadOnlyCollection<IMessageFlow> SeriviceBusFlows
            => new[] { ImportFactsFromErmFlow.Instance };

        public IReadOnlyCollection<IMessageFlow> SqlFlows
            => new IMessageFlow[] { AggregatesFlow.Instance, StatisticsFlow.Instance };

        public void ReportFlowLength(IMessageFlow flow, int length)
        {
            Action<int> reporter;
            if (_flowReporters.TryGetValue(flow.Id, out reporter))
            {
                reporter.Invoke(length);
            }

            throw new ArgumentException($"Поток не поддеривается: {flow.GetType().Name}", nameof(flow));
        }
    }
}