using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.ServiceBus.Messaging;

using NuClear.Messaging.Transports.ServiceBus;
using NuClear.Messaging.Transports.ServiceBus.API;

namespace NuClear.ValidationRules.Replication.Host.DI
{
    internal sealed class BatchingServiceBusMessageSender : IServiceBusMessageSender
    {
        private readonly ServiceBusMessageSender _senderImplementation;
        private const int BatchSize = 5000;

        public BatchingServiceBusMessageSender(ServiceBusMessageSender senderImplementation)
        {
            _senderImplementation = senderImplementation;
        }

        public bool TrySend(IEnumerable<BrokeredMessage> messages)
            => messages.Select((message, index) => Tuple.Create(message, index))
                       .GroupBy(tuple => tuple.Item2 / BatchSize, tuple => tuple.Item1)
                       .All(batch => _senderImplementation.TrySend(batch));
    }
}
