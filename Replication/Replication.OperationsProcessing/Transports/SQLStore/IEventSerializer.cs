using NuClear.OperationsProcessing.Transports.SQLStore.Final;
using NuClear.Replication.Core.API;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    public interface IEventSerializer
    {
        IEvent Deserialize(PerformedOperationFinalProcessing message);
        PerformedOperationFinalProcessing Serialize(IEvent @event);
    }
}
