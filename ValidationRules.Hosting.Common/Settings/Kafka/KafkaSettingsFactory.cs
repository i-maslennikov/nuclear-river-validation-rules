using System;
using System.Collections.Generic;
using System.Linq;

using Confluent.Kafka;

using Newtonsoft.Json;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.River.Hosting.Common.Settings;

namespace ValidationRules.Hosting.Common.Settings.Kafka
{
    public sealed class KafkaSettingsFactory : IKafkaSettingsFactory
    {
        private readonly Offset _offset;

        private readonly IReadOnlyDictionary<string, object> _defaultKafkaClientSpecificSettings =
            new Dictionary<string, object>
                {
                    ["socket.blocking.max.ms"] = 1,
                    ["fetch.wait.max.ms"] = 5,
                    ["fetch.error.backoff.ms"] = 5,
                    ["fetch.message.max.bytes"] = 10240,
                    ["queued.min.messages"] = 1000
                };

        private readonly Dictionary<IMessageFlow, KafkaConfigSettings> _flows2ConsumerSettingsMap;

        public KafkaSettingsFactory(
            IReadOnlyDictionary<IMessageFlow, string> messageFlows2CoonectionStringsMap,
            IEnvironmentSettings environmentSettings)
            : this(messageFlows2CoonectionStringsMap, environmentSettings, Offset.Invalid)
        {
        }

        public KafkaSettingsFactory(
            IReadOnlyDictionary<IMessageFlow, string> messageFlows2CoonectionStringsMap,
            IEnvironmentSettings environmentSettings,
            Offset offset)
        {
            _offset = offset;

            const string KafkaTargetTopicToken = "targetTopic";
            const string KafkaPollTimeoutToken = "pollTimeout";
            const string KafkaInfoTimeoutToken = "infoTimeout";

            _flows2ConsumerSettingsMap = new Dictionary<IMessageFlow, KafkaConfigSettings>();
            foreach (var entry in messageFlows2CoonectionStringsMap)
            {
                var messageFlow = entry.Key;
                var connectionString = entry.Value;

                var kafkaConfig = new KafkaConfigSettings();

                var configuredKafkaSettings = JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionString);
                if (!configuredKafkaSettings.TryGetValue(KafkaTargetTopicToken, out var rawtargetTopic))
                {
                    throw new InvalidOperationException($"Kafka config is invalid for message flow {messageFlow.GetType().Name}. Required parameter \"{KafkaTargetTopicToken}\" was not found. ConnectionString: {connectionString}");
                }

                kafkaConfig.Topic = (string)rawtargetTopic;
                kafkaConfig.PoolTimeout = !configuredKafkaSettings.TryGetValue(KafkaPollTimeoutToken, out object rawPollTimeout)
                                              ? TimeSpan.FromSeconds(5)
                                              : TimeSpan.Parse((string)rawPollTimeout);
                kafkaConfig.InfoTimeout = !configuredKafkaSettings.TryGetValue(KafkaInfoTimeoutToken, out object rawInfoTimeout)
                                              ? TimeSpan.FromSeconds(5)
                                              : TimeSpan.Parse((string)rawInfoTimeout);
                var explicitlyProcessedTokens = new[] { KafkaTargetTopicToken, KafkaPollTimeoutToken, KafkaInfoTimeoutToken };
                var kafkaClientSpecific = configuredKafkaSettings.Where(e => !explicitlyProcessedTokens.Contains(e.Key))
                                                                 .ToDictionary(x => x.Key, x => x.Value);

                kafkaClientSpecific["group.id"] = messageFlow.Id.ToString() + '-' + environmentSettings.EnvironmentName;

                foreach (var defaultSetting in _defaultKafkaClientSpecificSettings)
                {
                    if (kafkaClientSpecific.ContainsKey(defaultSetting.Key))
                    {
                        continue;
                    }

                    kafkaClientSpecific.Add(defaultSetting.Key, defaultSetting.Value);
                }

                _flows2ConsumerSettingsMap.Add(messageFlow, kafkaConfig);
            }
        }

        public IKafkaMessageFlowReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow)
        {
            if (!_flows2ConsumerSettingsMap.TryGetValue(messageFlow, out var kafkaConfig))
            {
                throw new ArgumentOutOfRangeException($"Can't create kafka receiver settings. Specified message flow \"{messageFlow.GetType().Name}\" doesn't has appropriate config");
            }

            return new KafkaMessageFlowReceiverSettings
            {
                Config = kafkaConfig.KafkaClientSpecific,
                TopicPartitionOffsets = new [] {  new TopicPartitionOffset(kafkaConfig.Topic, 0, _offset) },
                PollTimeout = kafkaConfig.PoolTimeout
            };
        }

        public IKafkaMessageFlowInfoSettings CreateInfoSettings(IMessageFlow messageFlow)
        {
            if (!_flows2ConsumerSettingsMap.TryGetValue(messageFlow, out var kafkaConfig))
            {
                throw new ArgumentOutOfRangeException($"Can't create kafka info settings. Specified message flow \"{messageFlow.GetType().Name}\" doesn't has appropriate config");
            }

            return new KafkaMessageFlowInfoSettings
            {
                Config = kafkaConfig.KafkaClientSpecific,
                TopicPartition = new TopicPartition(kafkaConfig.Topic, 0),
                InfoTimeout = kafkaConfig.InfoTimeout
            };
        }

        private sealed class KafkaConfigSettings
        {
            public string Topic { get; set; }
            public TimeSpan PoolTimeout { get; set; }
            public TimeSpan InfoTimeout { get; set; }
            public Dictionary<string, object> KafkaClientSpecific { get; set; }
        }
    }
}
