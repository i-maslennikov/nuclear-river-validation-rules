using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Telemetry
{
    public sealed class MessageEnqueuedOperationCountIdentity : TelemetryIdentityBase<MessageEnqueuedOperationCountIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Количество добавленных в очередь операций пересчёта состояния заказов"; }
        }
    }
}