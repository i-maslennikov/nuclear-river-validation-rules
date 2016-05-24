using System;
using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    [Obsolete("должен быть удалён")]
    public interface IEventDispatcher
    {
        IDictionary<IMessageFlow, IReadOnlyCollection<IEvent>> Dispatch(IReadOnlyCollection<IEvent> events);
    }
}