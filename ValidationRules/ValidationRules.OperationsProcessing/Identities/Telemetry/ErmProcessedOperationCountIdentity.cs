using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry
{
    public sealed class ErmProcessedOperationCountIdentity : TelemetryIdentityBase<ErmProcessedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество обработанных CUD операций"; }
        }
    }
}