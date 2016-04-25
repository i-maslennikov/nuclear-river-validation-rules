using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public sealed class ErmReceivedUseCaseCountIdentity : TelemetryIdentityBase<ErmReceivedUseCaseCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество принятых TUC из ERM (сообщений из SB)"; }
        }
    }
}