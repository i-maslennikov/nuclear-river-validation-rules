using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public class ServiceBusEventReceiver : MessageReceiverBase<ServiceBusEventMessage, IPerformedOperationsReceiverSettings>
    {
        private readonly ServiceBusMessageFlowReceiverFactory _serviceBusMessageFlowReceiverFactory;

        public ServiceBusEventReceiver(MessageFlowMetadata sourceFlowMetadata, IPerformedOperationsReceiverSettings messageReceiverSettings, ServiceBusMessageFlowReceiverFactory serviceBusMessageFlowReceiverFactory)
            : base(sourceFlowMetadata, messageReceiverSettings)
        {
            _serviceBusMessageFlowReceiverFactory = serviceBusMessageFlowReceiverFactory;
        }

        protected override IReadOnlyList<ServiceBusEventMessage> Peek()
            => _serviceBusMessageFlowReceiverFactory.Create(SourceFlowMetadata.MessageFlow)
                                                    .ReceiveBatch(MessageReceiverSettings.BatchSize)
                                                    .Select(x => new ServiceBusEventMessage(x))
                                                    .ToList();

        protected override void Complete(IEnumerable<ServiceBusEventMessage> successfullyProcessedMessages, IEnumerable<ServiceBusEventMessage> failedProcessedMessages)
            => _serviceBusMessageFlowReceiverFactory.Create(SourceFlowMetadata.MessageFlow)
                                                    .CompleteBatch(LockTokens(successfullyProcessedMessages), LockTokens(failedProcessedMessages));

        private IReadOnlyCollection<Guid> LockTokens(IEnumerable<ServiceBusEventMessage> messages)
            => messages.Select(x => x.BrokeredMessage.LockToken).ToList();
    }
}