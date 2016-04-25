using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public sealed class BitStatisticsEntityProcessedCountIdentity : TelemetryIdentityBase<BitStatisticsEntityProcessedCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество принятых записей статистики из Bit"; }
        }
    }
}