using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow
{
    public sealed class AmsFactsFlowTelemetryPublisher : IFlowTelemetryPublisher
    {
        private readonly ITelemetryPublisher _telemetryPublisher;

        public AmsFactsFlowTelemetryPublisher(ITelemetryPublisher telemetryPublisher)
        {
            _telemetryPublisher = telemetryPublisher;
        }

        public void Peeked(int count)
            => _telemetryPublisher.Publish<AmsFactsFlowPeekedEventCountIdentity>(count);

        public void Completed(int count)
            => _telemetryPublisher.Publish<AmsFactsFlowCompletedEventCountIdentity>(count);

        public void Failed(int count)
            => _telemetryPublisher.Publish<AmsFactsFlowFailedEventCountIdentity>(count);

        public void Delay(int delay)
            => _telemetryPublisher.Publish<AmsFactsFlowDelayIdentity>(delay);

        public sealed class AmsFactsFlowPeekedEventCountIdentity : TelemetryIdentityBase<AmsFactsFlowPeekedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(AmsFactsFlowPeekedEventCountIdentity);
        }

        public sealed class AmsFactsFlowCompletedEventCountIdentity : TelemetryIdentityBase<AmsFactsFlowCompletedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(AmsFactsFlowCompletedEventCountIdentity);
        }

        public sealed class AmsFactsFlowFailedEventCountIdentity : TelemetryIdentityBase<AmsFactsFlowFailedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(AmsFactsFlowFailedEventCountIdentity);
        }

        public sealed class AmsFactsFlowDelayIdentity : TelemetryIdentityBase<AmsFactsFlowDelayIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(AmsFactsFlowDelayIdentity);
        }
    }
}
