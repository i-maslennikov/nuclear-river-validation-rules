using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public sealed class StatisticsProcessedOperationCountIdentity : TelemetryIdentityBase<StatisticsProcessedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество обработанных операций пересчёта статистики"; }
        }
    }
}