using NuClear.Telemetry;

namespace NuClear.ValidationRules.OperationsProcessing.FactsFlow
{
    public sealed class FactsFlowReceiverTelemetryReporter : IReceiverTelemetryReporter
    {
        private readonly ITelemetryPublisher _telemetryPublisher;

        public FactsFlowReceiverTelemetryReporter(ITelemetryPublisher telemetryPublisher)
        {
            _telemetryPublisher = telemetryPublisher;
        }

        public void Peeked(int count)
        {
            _telemetryPublisher.Publish<FactsFlowPeekedEventCountIdentity>(count);
        }

        public void Completed(int count)
        {
            _telemetryPublisher.Publish<FactsFlowCompletedEventCountIdentity>(count);
        }

        public void Failed(int count)
        {
            _telemetryPublisher.Publish<FactsFlowFailedEventCountIdentity>(count);
        }

        public sealed class FactsFlowPeekedEventCountIdentity : TelemetryIdentityBase<FactsFlowPeekedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(FactsFlowPeekedEventCountIdentity);
        }

        public sealed class FactsFlowCompletedEventCountIdentity : TelemetryIdentityBase<FactsFlowCompletedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(FactsFlowCompletedEventCountIdentity);
        }

        public sealed class FactsFlowFailedEventCountIdentity : TelemetryIdentityBase<FactsFlowFailedEventCountIdentity>
        {
            public override int Id => 0;
            public override string Description => nameof(FactsFlowFailedEventCountIdentity);
        }
    }
}