using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Replication.OperationsProcessing.Telemetry;
using NuClear.Telemetry;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class ServiceBusOperationsReceiverTelemetryDecorator : IMessageReceiver
    {
        private readonly IMessageReceiver _receiver;
        private readonly ITelemetryPublisher _telemetryPublisher;

        public ServiceBusOperationsReceiverTelemetryDecorator(ServiceBusOperationsReceiver receiver, ITelemetryPublisher telemetryPublisher)
        {
            _receiver = receiver;
            _telemetryPublisher = telemetryPublisher;
        }

        public IReadOnlyList<IMessage> Peek()
        {
            using (Probe.Create("Peek Erm Operations"))
            {
                var messages = _receiver.Peek();

                var serviceBusMessageCount = messages.Cast<ServiceBusPerformedOperationsMessage>().Sum(x => x.Operations.Count());
                _telemetryPublisher.Publish<ErmReceivedUseCaseCountIdentity>(serviceBusMessageCount);

                return messages;
            }
        }

        public void Complete(IEnumerable<IMessage> successfullyProcessedMessages, IEnumerable<IMessage> failedProcessedMessages)
        {
            using (Probe.Create("Complete Erm Operations"))
            {
                _receiver.Complete(successfullyProcessedMessages, failedProcessedMessages);
            }
        }
    }
}