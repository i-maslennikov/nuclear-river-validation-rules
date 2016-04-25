using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public sealed class StatisticsProcessingDelayIdentity : TelemetryIdentityBase<StatisticsProcessingDelayIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Интервал времени между постановкой операции пересчёта статистики в очередь и до её обработки, мс"; }
        }
    }
}