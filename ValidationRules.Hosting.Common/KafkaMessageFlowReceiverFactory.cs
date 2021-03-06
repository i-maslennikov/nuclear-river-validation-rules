﻿
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.Tracing.API;

using ValidationRules.Hosting.Common.Settings.Kafka;

namespace ValidationRules.Hosting.Common
{
    public sealed class KafkaMessageFlowReceiverFactory : IKafkaMessageFlowReceiverFactory
    {
        private readonly ITracer _tracer;
        private readonly IKafkaSettingsFactory _kafkaSettingsFactory;

        public KafkaMessageFlowReceiverFactory(ITracer tracer, IKafkaSettingsFactory kafkaSettingsFactory)
        {
            _tracer = tracer;
            _kafkaSettingsFactory = kafkaSettingsFactory;
        }

        public IKafkaMessageFlowReceiver Create(IMessageFlow messageFlow)
        {
            var settings = _kafkaSettingsFactory.CreateReceiverSettings(messageFlow);
            return new KafkaTopicConsumerWrapper(settings, _tracer);
        }
    }
}