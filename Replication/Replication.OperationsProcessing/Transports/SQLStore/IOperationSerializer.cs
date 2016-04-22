using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public interface IOperationSerializer
    {
        IOperation Deserialize(PerformedOperationFinalProcessing operation);
        PerformedOperationFinalProcessing Serialize(IOperation operation);
    }
}
