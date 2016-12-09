using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.Core;
using NuClear.Telemetry.Probing;

namespace NuClear.ValidationRules.OperationsProcessing.Transports
{
    public sealed class BatchingServiceBusMessageReceiverTelemetryDecorator : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;

        public BatchingServiceBusMessageReceiverTelemetryDecorator(ServiceBusMessageReceiver receiver)
        {
            _receiver = receiver;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek messages from ServiceBus"))
            {
                var messages = _receiver.Peek();
                return messages;
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete ServiceBus messages"))
            {
                foreach(var batch in successfullyProcessedMessages.CreateBatches(500))
                {
                    _receiver.Complete(batch, Array.Empty<IMessage>());
                }

                foreach (var batch in failedProcessedMessages.CreateBatches(500))
                {
                    _receiver.Complete(Array.Empty<IMessage>(), batch);
                }
            }
        }
    }
}
