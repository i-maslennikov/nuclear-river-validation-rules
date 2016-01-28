using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Messaging.API.Flows;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IOperationSender
    {
        void Push<TOperation, TFlow>(IEnumerable<TOperation> operations, TFlow targetFlow)
            where TFlow : MessageFlowBase<TFlow>, new()
            where TOperation : AggregateOperation;
    }
}