using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.Kafka;
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
                _publisher.Peeked(messages.Count);
                return messages;
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete Kafka messages"))
            {
                var succeeded = successfullyProcessedMessages.Cast<KafkaMessage>().ToList();
                var failed = failedProcessedMessages.Cast<KafkaMessage>().ToList();

                _receiver.Complete(succeeded, failed);

                _publisher.Completed(succeeded.Count);
                _publisher.Failed(failed.Count);
            }
        }
    }
}
