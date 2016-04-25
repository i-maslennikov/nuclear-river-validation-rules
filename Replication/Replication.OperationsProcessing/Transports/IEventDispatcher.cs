using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core.API;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IEventDispatcher
    {
        IDictionary<IMessageFlow, IReadOnlyCollection<IEvent>> Dispatch(IReadOnlyCollection<IEvent> events);
    }
}