using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry
{
    public sealed class PrimaryProcessingDelayIdentity : TelemetryIdentityBase<PrimaryProcessingDelayIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Интервал времени между завершением TUC в ERM и его обработкой в AS, мс"; }
        }
    }
}