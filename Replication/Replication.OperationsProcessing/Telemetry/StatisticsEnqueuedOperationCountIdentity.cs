using NuClear.Telemetry;

namespace NuClear.Replication.OperationsProcessing.Telemetry
{
    public sealed class StatisticsEnqueuedOperationCountIdentity : TelemetryIdentityBase<StatisticsEnqueuedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество операций пересчёта статистики, добавленных в очередь на обработку"; }
        }
    }
}