using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.Replication.Core;
using NuClear.Telemetry.Probing;

namespace NuClear.ValidationRules.OperationsProcessing.Transports.Kafka
{
    public sealed class BatchingKafkaReceiverTelemetryDecorator<TReceiverActionReporter> : IMessageReceiver
        where TReceiverActionReporter : IFlowTelemetryPublisher
    {
        private readonly IMessageReceiver _receiver;
        private readonly IFlowTelemetryPublisher _publisher;

        public BatchingKafkaReceiverTelemetryDecorator(KafkaReceiver receiver, TReceiverActionReporter publisher)
        {
            _receiver = receiver;
            _publisher = publisher;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek messages from Kafka"))
            {
                var messages = _receiver.Peek();
                _publisher.Peeked(messages.Cast<KafkaMessage>().Sum(x => x.Messages.Count));
                return messages;
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete Kafka messages"))
            {
                foreach (var batch in successfullyProcessedMessages.CreateBatches(500))
                {
                    _receiver.Complete(batch, Array.Empty<IMessage>());
                    _publisher.Completed(batch.Cast<KafkaMessage>().Sum(x => x.Messages.Count));
                }

                foreach (var batch in failedProcessedMessages.CreateBatches(500))
                {
                    _receiver.Complete(Array.Empty<IMessage>(), batch);
                    _publisher.Failed(batch.Cast<KafkaMessage>().Sum(x => x.Messages.Count));
                }
            }
        }
    }
}
