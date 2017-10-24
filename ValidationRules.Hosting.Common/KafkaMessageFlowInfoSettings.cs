using System;
using System.Collections.Generic;

using Confluent.Kafka;

namespace ValidationRules.Hosting.Common
{
    internal sealed class KafkaMessageFlowInfoSettings : IKafkaMessageFlowInfoSettings
    {
        public Dictionary<string, object> Config { get; set; }
        public TopicPartition TopicPartition { get; set; }
        public TimeSpan InfoTimeout { get; set; }
    }
}