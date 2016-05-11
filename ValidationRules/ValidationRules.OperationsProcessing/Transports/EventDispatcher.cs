using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Transports
{
    public sealed class EventDispatcher : IEventDispatcher
    {
        public IDictionary<IMessageFlow, IReadOnlyCollection<IEvent>> Dispatch(IReadOnlyCollection<IEvent> operations)
        {
            return new Dictionary<IMessageFlow, IReadOnlyCollection<IEvent>>
                {
                    { CommonEventsFlow.Instance, operations.ToArray() },
                };
        }
    }
}
