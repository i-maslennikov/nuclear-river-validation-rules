using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class ServiceBusEventReceiverTelemetryDecorator : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;

        public ServiceBusEventReceiverTelemetryDecorator(ServiceBusEventReceiver receiver)
        {
            _receiver = receiver;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek Aggregate Operations"))
            {
                var result = _receiver.Peek();
                return result;
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete Aggregate Operations"))
            {
                _receiver.Complete(successfullyProcessedMessages, failedProcessedMessages);
            }
        }
    }
}