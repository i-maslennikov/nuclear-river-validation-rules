using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.OperationsProcessing.Transports.Kafka
{
    public sealed class KafkaMessageFlowReceiverFactory : IKafkaMessageFlowReceiverFactory
    {
        private readonly ITracer _tracer;
        private readonly IAmsSettingsFactory _amsSettingsFactory;

        public KafkaMessageFlowReceiverFactory(ITracer tracer, IAmsSettingsFactory amsSettingsFactory)
        {
            _tracer = tracer;
            _amsSettingsFactory = amsSettingsFactory;
        }

        public IKafkaMessageFlowReceiver Create(IMessageFlow messageFlow)
        {
            var settings = _amsSettingsFactory.CreateReceiverSettings(messageFlow);
            return new KafkaMessageFlowReceiver(settings, _tracer);
        }
    }
}