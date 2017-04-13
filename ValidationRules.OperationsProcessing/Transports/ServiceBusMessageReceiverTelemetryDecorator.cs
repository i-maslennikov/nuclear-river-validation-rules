using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.Core;
using NuClear.Telemetry.Probing;

namespace NuClear.ValidationRules.OperationsProcessing.Transports
{
    public sealed class BatchingServiceBusMessageReceiverTelemetryDecorator<TReceiverActionReporter> : IMessageReceiver
        where TReceiverActionReporter : IReceiverTelemetryReporter
    {
        private readonly IMessageReceiver _receiver;
        private readonly IReceiverTelemetryReporter _reporter;

        public BatchingServiceBusMessageReceiverTelemetryDecorator(ServiceBusMessageReceiver receiver, TReceiverActionReporter reporter)
        {
            _receiver = receiver;
            _reporter = reporter;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek messages from ServiceBus"))
            {
                var messages = _receiver.Peek();
                _reporter.Peeked(messages.Count);
                return messages;
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete ServiceBus messages"))
            {
                foreach (var batch in successfullyProcessedMessages.CreateBatches(500))
                {
                    _receiver.Complete(batch, Array.Empty<IMessage>());
                    _reporter.Completed(batch.Count);
                }

                foreach (var batch in failedProcessedMessages.CreateBatches(500))
                {
                    _receiver.Complete(Array.Empty<IMessage>(), batch);
                    _reporter.Failed(batch.Count);
                }
            }
        }
    }
}
