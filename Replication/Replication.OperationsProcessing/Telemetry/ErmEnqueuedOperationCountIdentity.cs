using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public sealed class ErmEnqueuedOperationCountIdentity : TelemetryIdentityBase<ErmEnqueuedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество взятых на обработку CUD операций"; }
        }
    }
}