using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public class FinalProcessingAggregateQueueLengthIdentity : TelemetryIdentityBase<FinalProcessingAggregateQueueLengthIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Размер очереди ETL2 (агрегаты)"; }
        }
    }
}