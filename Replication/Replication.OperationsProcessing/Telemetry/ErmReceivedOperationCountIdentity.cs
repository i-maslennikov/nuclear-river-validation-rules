using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public sealed class ErmReceivedOperationCountIdentity : TelemetryIdentityBase<ErmReceivedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество принятых CUD операций из ERM"; }
        }
    }
}