using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.River.Common.Metadata;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public interface IEventSerializer
    {
        IEvent Deserialize(PerformedOperationFinalProcessing message);
        PerformedOperationFinalProcessing Serialize(IEvent @event);
    }
}
