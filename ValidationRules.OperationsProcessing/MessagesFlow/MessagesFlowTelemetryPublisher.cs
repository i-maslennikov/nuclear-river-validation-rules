using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.MessagesFlow
{
    public sealed class MessagesFlowTelemetryPublisher : IFlowTelemetryPublisher
    {
        private readonly ITelemetryPublisher _telemetryPublisher;

        public MessagesFlowTelemetryPublisher(ITelemetryPublisher telemetryPublisher)
        {
            _telemetryPublisher = telemetryPublisher;
        }

        public void Peeked(int count)
            => _telemetryPublisher.Publish<MessagesFlowPeekedEventCountIdentity>(count);

        public void Completed(int count)
            => _telemetryPublisher.Publish<MessagesFlowCompletedEventCountIdentity>(count);

        public void Failed(int count)
            => _telemetryPublisher.Publish<MessagesFlowFailedEventCountIdentity>(count);

        public void Delay(int delay)
            => _telemetryPublisher.Publish<MessagesFlowDelayIdentity>(delay);

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

        public sealed class MessagesFlowDelayIdentity : TelemetryIdentityBase<MessagesFlowDelayIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(MessagesFlowDelayIdentity);
        }
    }
}