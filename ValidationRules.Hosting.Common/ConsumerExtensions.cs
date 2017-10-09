using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Confluent.Kafka;

using Newtonsoft.Json;

namespace ValidationRules.Hosting.Common
{
    public static class ConsumerExtensions
    {
        public static readonly AmsSettings Settings = new AmsSettings();

        public static long GetPartitionSize(this Consumer consumer, int partition = 0)
        {
            var topicPartition = new TopicPartition(Settings.Topic, partition);
            var offsets = consumer.QueryWatermarkOffsets(topicPartition, Settings.Timeout);
            return offsets.High;
        }

        public static bool TryGetLatestMessage(this Consumer consumer, int partition, out Message message)
        {
            var topicPartition = new TopicPartition(Settings.Topic, partition);
            var offsets = consumer.QueryWatermarkOffsets(topicPartition, Settings.Timeout);
            consumer.Assign(new[] { new TopicPartitionOffset(topicPartition, offsets.High - 1) });

            return consumer.Consume(out message, Settings.Timeout);
        }

        public sealed class AmsSettings
        {
            public AmsSettings()
            {
                var connectionString = ConfigurationManager.ConnectionStrings["Ams"].ConnectionString;
                Config = JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionString);
                Config.Add("group.id", Guid.NewGuid().ToString());

                Topic = ConfigurationManager.AppSettings["AmsFactsTopics"].Split(',').First();
            }

            public Dictionary<string, object> Config { get; }
            public string Topic { get; }
            public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(5);
        }
    }
}