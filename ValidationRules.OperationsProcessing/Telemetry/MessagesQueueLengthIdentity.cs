using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.Telemetry
{
    public class MessagesQueueLengthIdentity : TelemetryIdentityBase<MessagesQueueLengthIdentity>
    {
        public override int Id
        {
            get { return 0; }
        }

        public override string Description
        {
            get { return "Размер очереди на вызов правил валидации"; }
        }
    }
}