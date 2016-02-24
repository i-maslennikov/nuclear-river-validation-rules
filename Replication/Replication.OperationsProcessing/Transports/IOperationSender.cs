using System.Collections.Generic;

using NuClear.Messaging.API.Flows;
using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IOperationSender
    {
        void Push<TOperation, TFlow>(IEnumerable<TOperation> operations, TFlow targetFlow)
            where TFlow : MessageFlowBase<TFlow>, new()
            where TOperation : AggregateOperation;
    }
}