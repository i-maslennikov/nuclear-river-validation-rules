using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry
{
    public sealed class AggregateProcessedOperationCountIdentity : TelemetryIdentityBase<AggregateProcessedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество обработанных операций над агрегатами"; }
        }
    }
}