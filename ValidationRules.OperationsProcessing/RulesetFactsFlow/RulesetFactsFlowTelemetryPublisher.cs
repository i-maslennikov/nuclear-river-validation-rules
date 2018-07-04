using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow
{
    public sealed class RulesetFactsFlowTelemetryPublisher : IFlowTelemetryPublisher
    {
        private readonly ITelemetryPublisher _telemetryPublisher;

        public RulesetFactsFlowTelemetryPublisher(ITelemetryPublisher telemetryPublisher)
        {
            _telemetryPublisher = telemetryPublisher;
        }

        public void Peeked(int count)
            => _telemetryPublisher.Publish<RulesetFactsFlowPeekedEventCountIdentity>(count);

        public void Completed(int count)
            => _telemetryPublisher.Publish<RulesetFactsFlowCompletedEventCountIdentity>(count);

        public void Failed(int count)
            => _telemetryPublisher.Publish<RulesetFactsFlowFailedEventCountIdentity>(count);

        public void Delay(int delay)
            => _telemetryPublisher.Publish<RulesetFactsFlowDelayIdentity>(delay);

        public sealed class RulesetFactsFlowPeekedEventCountIdentity : TelemetryIdentityBase<RulesetFactsFlowPeekedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(RulesetFactsFlowPeekedEventCountIdentity);
        }

        public sealed class RulesetFactsFlowCompletedEventCountIdentity : TelemetryIdentityBase<RulesetFactsFlowCompletedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(RulesetFactsFlowCompletedEventCountIdentity);
        }

        public sealed class RulesetFactsFlowFailedEventCountIdentity : TelemetryIdentityBase<RulesetFactsFlowFailedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(RulesetFactsFlowFailedEventCountIdentity);
        }

        public sealed class RulesetFactsFlowDelayIdentity : TelemetryIdentityBase<RulesetFactsFlowDelayIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(RulesetFactsFlowDelayIdentity);
        }
    }
}