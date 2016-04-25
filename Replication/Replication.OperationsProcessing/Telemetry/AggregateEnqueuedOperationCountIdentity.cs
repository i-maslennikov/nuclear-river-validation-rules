using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public sealed class AggregateEnqueuedOperationCountIdentity : TelemetryIdentityBase<AggregateEnqueuedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество операций над агрегатами, добавленных в очередь на обработку"; }
        }
    }
}