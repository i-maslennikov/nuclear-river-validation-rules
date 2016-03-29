using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IOperationDispatcher
    {
        IDictionary<IMessageFlow, IReadOnlyCollection<IOperation>> Dispatch(IEnumerable<IOperation> operations);
    }
}