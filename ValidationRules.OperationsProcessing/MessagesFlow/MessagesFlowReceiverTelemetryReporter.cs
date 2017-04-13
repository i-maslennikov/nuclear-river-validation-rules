using NuClear.Telemetry;
using NuClear.ValidationRules.OperationsProcessing.Transports;

namespace NuClear.ValidationRules.OperationsProcessing.MessagesFlow
{
    public sealed class MessagesFlowReceiverTelemetryReporter : IReceiverTelemetryReporter
    {
        private readonly ITelemetryPublisher _telemetryPublisher;

        public MessagesFlowReceiverTelemetryReporter(ITelemetryPublisher telemetryPublisher)
        {
            _telemetryPublisher = telemetryPublisher;
        }

        public void Peeked(int count)
        {
            _telemetryPublisher.Publish<MessagesFlowPeekedEventCountIdentity>(count);
        }

        public void Completed(int count)
        {
            _telemetryPublisher.Publish<MessagesFlowCompletedEventCountIdentity>(count);
        }

        public void Failed(int count)
        {
            _telemetryPublisher.Publish<MessagesFlowFailedEventCountIdentity>(count);
        }

        public sealed class MessagesFlowPeekedEventCountIdentity : TelemetryIdentityBase<MessagesFlowPeekedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(MessagesFlowPeekedEventCountIdentity);
        }

        public sealed class MessagesFlowCompletedEventCountIdentity : TelemetryIdentityBase<MessagesFlowCompletedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(MessagesFlowCompletedEventCountIdentity);
        }

        public sealed class MessagesFlowFailedEventCountIdentity : TelemetryIdentityBase<MessagesFlowFailedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(MessagesFlowFailedEventCountIdentity);
        }
    }
}