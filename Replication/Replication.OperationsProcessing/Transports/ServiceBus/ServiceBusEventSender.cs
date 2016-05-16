using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories;
using NuClear.Telemetry.Probing;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class ServiceBusEventSender : IEventSender
    {
        private readonly IXmlEventSerializer _serializer;
        private readonly ServiceBusMessageSenderFactory _factory;

        public ServiceBusEventSender(
            IXmlEventSerializer serializer,
            ServiceBusMessageSenderFactory factory)
        {
            _serializer = serializer;
            _factory = factory;
        }

        public void Push<TEvent, TFlow>(TFlow targetFlow, IReadOnlyCollection<TEvent> events)
            where TFlow : IMessageFlow
            where TEvent : IEvent
        {
            if (!events.Any())
            {
                return;
            }

            using (Probe.Create($"Send {typeof(TEvent).Name}"))
            {
                var sender = _factory.Create(targetFlow);
                var transportMessages = events.Select(x => ServiceBusEventMessage.Create(Guid.NewGuid(), _serializer.Serialize(x)));

                // FAIL: ServiceBus (целиком или реализация клиента?) не поддерживает отправку более 100 сообщений в одной транзакции с одной стороны и не позволяет убрать транзакцию совсем с другой.
                // т.е. TransactionScope есть, но транзакционности - нет. Это может привести к дублированию сообщений в топике (различные по идентификатору, но одинаковые по содержимому).

                foreach (var batch in CreateBatches(transportMessages, 100))
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                    {
                        if (!sender.TrySend(batch.Select(x => x.BrokeredMessage)))
                        {
                            throw new Exception("Can not send events");
                        }

                        scope.Complete();
                    }
                }
            }
        }

        private static IEnumerable<IEnumerable<T>> CreateBatches<T>(IEnumerable<T> enumerable, int count)
            => enumerable.Select((item, index) => new { Item = item, BatchNumber = index / count }).GroupBy(x => x.BatchNumber, x => x.Item);
    }
}