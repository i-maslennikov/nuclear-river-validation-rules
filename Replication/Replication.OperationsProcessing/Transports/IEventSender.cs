using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    [Obsolete("должен быть заменён на IEventLogger из пакета operation-logging")]
    public interface IEventSender
    {
        void Push<TEvent, TFlow>(TFlow targetFlow, IReadOnlyCollection<TEvent> events)
            where TFlow : IMessageFlow
            where TEvent : IEvent;
    }
}