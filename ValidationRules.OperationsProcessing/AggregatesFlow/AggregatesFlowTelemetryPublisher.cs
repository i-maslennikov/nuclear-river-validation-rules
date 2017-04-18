using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.AggregatesFlow
{
    public sealed class AggregatesFlowTelemetryPublisher : IFlowTelemetryPublisher
    {
        private readonly ITelemetryPublisher _telemetryPublisher;

        public AggregatesFlowTelemetryPublisher(ITelemetryPublisher telemetryPublisher)
        {
            _telemetryPublisher = telemetryPublisher;
        }

        public void Peeked(int count)
            => _telemetryPublisher.Publish<AggregatesFlowPeekedEventCountIdentity>(count);

        public void Completed(int count)
            => _telemetryPublisher.Publish<AggregatesFlowCompletedEventCountIdentity>(count);

        public void Failed(int count)
            => _telemetryPublisher.Publish<AggregatesFlowFailedEventCountIdentity>(count);

        public void Delay(int delay)
            => _telemetryPublisher.Publish<AggregatesFlowDelayIdentity>(delay);

        public sealed class AggregatesFlowPeekedEventCountIdentity : TelemetryIdentityBase<AggregatesFlowPeekedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(AggregatesFlowPeekedEventCountIdentity);
        }

        public sealed class AggregatesFlowCompletedEventCountIdentity : TelemetryIdentityBase<AggregatesFlowCompletedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(AggregatesFlowCompletedEventCountIdentity);
        }

        public sealed class AggregatesFlowFailedEventCountIdentity : TelemetryIdentityBase<AggregatesFlowFailedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(AggregatesFlowFailedEventCountIdentity);
        }

        public sealed class AggregatesFlowDelayIdentity : TelemetryIdentityBase<AggregatesFlowDelayIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(AggregatesFlowDelayIdentity);
        }
    }
}