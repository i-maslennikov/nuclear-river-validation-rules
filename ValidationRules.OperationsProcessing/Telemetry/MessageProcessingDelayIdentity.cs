using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Telemetry
{
    public sealed class MessageProcessingDelayIdentity : TelemetryIdentityBase<MessageProcessingDelayIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Интервал времени между постановкой операции пересчёта состояния заказа в очередь и до её обработки, мс"; }
        }
    }
}