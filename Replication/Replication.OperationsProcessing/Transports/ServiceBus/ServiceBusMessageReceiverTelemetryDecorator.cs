using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class ServiceBusMessageReceiverTelemetryDecorator : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;

        public ServiceBusMessageReceiverTelemetryDecorator(ServiceBusMessageReceiver receiver)
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
                _receiver.Complete(successfullyProcessedMessages, failedProcessedMessages);
            }
        }
    }
}