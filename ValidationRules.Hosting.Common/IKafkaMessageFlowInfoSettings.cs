using System;
using System.Collections.Generic;

using Confluent.Kafka;

namespace ValidationRules.Hosting.Common
{
    public interface IKafkaMessageFlowInfoSettings
    {
        Dictionary<string, object> Config { get; }
        TopicPartition TopicPartition { get; }
        TimeSpan InfoTimeout { get; }
    }
}