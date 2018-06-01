using System.Linq;

using Confluent.Kafka;

using NuClear.Messaging.API.Flows;

using ValidationRules.Hosting.Common.Settings.Kafka;

namespace ValidationRules.Hosting.Common
{
    public sealed class KafkaMessageFlowInfoProvider
    {
        private readonly IKafkaSettingsFactory _kafkaSettingsFactory;

        public KafkaMessageFlowInfoProvider(IKafkaSettingsFactory kafkaSettingsFactory)
        {
            _kafkaSettingsFactory = kafkaSettingsFactory;
        }

        public long GetFlowSize(IMessageFlow messageFlow)
        {
            var settings = _kafkaSettingsFactory.CreateInfoSettings(messageFlow);

            using (var consumer = new Consumer(settings.Config))
            {
                var offsets = consumer.QueryWatermarkOffsets(settings.TopicPartition, settings.InfoTimeout);
                return offsets.High;
            }
        }

        public long GetFlowProcessedSize(IMessageFlow messageFlow)
        {
            var settings = _kafkaSettingsFactory.CreateInfoSettings(messageFlow);
            using (var consumer = new Consumer(settings.Config))
            {
                var committedOffset = consumer.Committed(new[] { settings.TopicPartition }, settings.InfoTimeout).First();
                return committedOffset.Offset;
            }
        }

        public bool TryGetFlowLastMessage(IMessageFlow messageFlow, out Message message)
        {
            var settings = _kafkaSettingsFactory.CreateInfoSettings(messageFlow);

            using (var consumer = new Consumer(settings.Config))
            {
                var offsets = consumer.QueryWatermarkOffsets(settings.TopicPartition, settings.InfoTimeout);
                consumer.Assign(new[] { new TopicPartitionOffset(settings.TopicPartition, offsets.High - 1) });

                return consumer.Consume(out message, settings.InfoTimeout);
            }
        }
    }
}