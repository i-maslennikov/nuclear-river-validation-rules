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
        where TReceiverActionReporter : IFlowTelemetryPublisher
    {
        private readonly IMessageReceiver _receiver;
        private readonly IFlowTelemetryPublisher _publisher;

        public BatchingServiceBusMessageReceiverTelemetryDecorator(ServiceBusMessageReceiver receiver, TReceiverActionReporter publisher)
        {
            _receiver = receiver;
            _publisher = publisher;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek messages from ServiceBus"))
            {
                var messages = _receiver.Peek();
                _publisher.Peeked(messages.Count);
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
                    _publisher.Completed(batch.Count);
                }

                foreach (var batch in failedProcessedMessages.CreateBatches(500))
                {
                    _receiver.Complete(Array.Empty<IMessage>(), batch);
                    _publisher.Failed(batch.Count);
                }
            }
        }
    }
}
