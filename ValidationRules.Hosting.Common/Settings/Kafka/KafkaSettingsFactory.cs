using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Confluent.Kafka;

using Newtonsoft.Json;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.Storage.Connections;

namespace ValidationRules.Hosting.Common
{
    public sealed class KafkaSettingsFactory : IKafkaSettingsFactory
    {
        private static readonly string[] Topics = ConfigFileSetting.String.Required("AmsFactsTopics").Value.Split(',');
        private static readonly TimeSpan PollTimeout = TimeSpan.Parse(ConfigFileSetting.String.Optional("AmsPollTimeout", "00:00:05").Value, CultureInfo.InvariantCulture);
        private static readonly TimeSpan InfoTimeout = TimeSpan.FromSeconds(5);

        private readonly Dictionary<string, object> _consumerConfig;
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly Offset _offset;

        public KafkaSettingsFactory(IConnectionStringSettings connectionStringSettings, IEnvironmentSettings environmentSettings)
            : this(connectionStringSettings, environmentSettings, Offset.Invalid) { }

        public KafkaSettingsFactory(IConnectionStringSettings connectionStringSettings, IEnvironmentSettings environmentSettings, Offset offset)
        {
            _environmentSettings = environmentSettings;
            _offset = offset;

            var connectionString = connectionStringSettings.GetConnectionString(AmsConnectionStringIdentity.Instance);
            var baseConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionString);
            _consumerConfig = new Dictionary<string, object>(baseConfig)
            {
                // performance tricks from denis
                { "socket.blocking.max.ms", 1 },
                { "fetch.wait.max.ms", 5 },
                { "fetch.error.backoff.ms", 5 },
                { "fetch.message.max.bytes", 10240 },
                { "queued.min.messages", 1000 },
            };
        }

        public IKafkaMessageFlowReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow)
        {
            var messageFlowConfig = new Dictionary<string, object>(_consumerConfig)
            {
                { "group.id", messageFlow.Id.ToString() + '-' + _environmentSettings.EnvironmentName }
            };

            return new KafkaMessageFlowReceiverSettings
            {
                Config = messageFlowConfig,
                TopicPartitionOffsets = Topics.Select(x => new TopicPartitionOffset(x, 0, _offset)),
                PollTimeout = PollTimeout
            };
        }

        public IKafkaMessageFlowInfoSettings CreateInfoSettings(IMessageFlow messageFlow)
        {
            var messageFlowConfig = new Dictionary<string, object>(_consumerConfig)
            {
                { "group.id", messageFlow.Id.ToString() + '-' + _environmentSettings.EnvironmentName }
            };

            return new KafkaMessageFlowInfoSettings
            {
                Config = messageFlowConfig,
                TopicPartition = new TopicPartition(Topics[0], 0),
                InfoTimeout = InfoTimeout
            };
        }

        private sealed class KafkaMessageFlowReceiverSettings : IKafkaMessageFlowReceiverSettings
        {
            public Dictionary<string, object> Config { get; set; }

            public IEnumerable<TopicPartitionOffset> TopicPartitionOffsets { get; set; }

            public TimeSpan PollTimeout { get; set; }
        }
    }
}
