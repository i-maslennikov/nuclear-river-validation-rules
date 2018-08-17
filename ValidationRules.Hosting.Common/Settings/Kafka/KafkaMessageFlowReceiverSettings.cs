using System;
using System.Collections.Generic;

using Confluent.Kafka;
using NuClear.Messaging.Transports.Kafka;

namespace ValidationRules.Hosting.Common.Settings.Kafka
{
    public sealed class KafkaMessageFlowReceiverSettings : IKafkaMessageFlowReceiverSettings
    {
        public Dictionary<string, object> Config { get; set; }

        public IEnumerable<TopicPartitionOffset> TopicPartitionOffsets { get; set; }

        public TimeSpan PollTimeout { get; set; }
    }
}
