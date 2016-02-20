using System.Collections.Generic;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IOperationSender<in TOperation>
        where TOperation : IOperation
    {
        void Push(IEnumerable<TOperation> operations);
    }
}