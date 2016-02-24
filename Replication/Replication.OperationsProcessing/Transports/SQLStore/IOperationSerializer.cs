using NuClear.Messaging.API.Flows;
using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public interface IOperationSerializer<TOperation>
        where TOperation : IOperation
    {
        TOperation Deserialize(PerformedOperationFinalProcessing operation);
        PerformedOperationFinalProcessing Serialize(TOperation operation, IMessageFlow targetFlow);
    }
}
