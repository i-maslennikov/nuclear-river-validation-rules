using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Telemetry
{
    public sealed class MessageProcessedOperationCountIdentity : TelemetryIdentityBase<MessageProcessedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество обработанных операций пересчёта состояния заказов"; }
        }
    }
}