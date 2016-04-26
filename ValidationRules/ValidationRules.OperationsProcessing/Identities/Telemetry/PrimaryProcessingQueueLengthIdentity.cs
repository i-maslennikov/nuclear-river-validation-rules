using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry
{
    public class PrimaryProcessingQueueLengthIdentity : TelemetryIdentityBase<PrimaryProcessingQueueLengthIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Размер очереди ETL1 (число TUC в SB)"; }
        }
    }
}