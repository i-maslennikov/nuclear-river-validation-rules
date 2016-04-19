using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Replication.OperationsProcessing.Transports;
using NuClear.River.Common.Metadata.Model;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Transports
{
    public sealed class OperationDispatcher : IOperationDispatcher
    {
        public IDictionary<IMessageFlow, IReadOnlyCollection<IOperation>> Dispatch(IEnumerable<IOperation> operations)
        {
            return new Dictionary<IMessageFlow, IReadOnlyCollection<IOperation>>
                {
                    { AggregatesFlow.Instance, operations.ToArray() },
                };
        }
    }
}
