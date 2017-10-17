using System;
using System.Collections.Generic;
using System.Globalization;

using Confluent.Kafka;

using Newtonsoft.Json;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.Transports.Kafka;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace ValidationRules.Hosting.Common
{
    public sealed class AmsSettingsFactory : IAmsSettingsFactory
    {
        private static readonly IEnumerable<string> Topics = ConfigFileSetting.String.Required("AmsFactsTopics").Value.Split(',');
        private static readonly TimeSpan PollTimeout = TimeSpan.Parse(ConfigFileSetting.String.Optional("AmsPollTimeout", "00:00:05").Value, CultureInfo.InvariantCulture);

        private readonly Dictionary<string, object> _config;
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly Offset _offset;

        public AmsSettingsFactory(IConnectionStringSettings connectionStringSettings, IEnvironmentSettings environmentSettings, Offset offset)
        {
            _environmentSettings = environmentSettings;
            _offset = offset;

            var connectionString = connectionStringSettings.GetConnectionString(AmsConnectionStringIdentity.Instance);
            var baseConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionString);
            _config = new Dictionary<string, object>(baseConfig)
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
            if (AmsFactsFlow.Instance.Equals(messageFlow))
            {
                return new KafkaMessageFlowReceiverSettings
                {
                    GroupId = messageFlow.Id.ToString() + '-' + _environmentSettings.EnvironmentName,
                    Config = _config,
                    Offset = _offset,
                    Topics = Topics,
                    Partition = 0,
                    PollTimeout = PollTimeout
                };
            }

            throw new ArgumentException($"Flow '{messageFlow.Description}' settings for Kafka are undefined");
        }

        private sealed class KafkaMessageFlowReceiverSettings : IKafkaMessageFlowReceiverSettings
        {
            public string GroupId { get; set; }
            public Dictionary<string, object> Config { get; set; }
            public Offset Offset { get; set; }

            public IEnumerable<string> Topics { get; set; }
            public int Partition { get; set; }
            public TimeSpan PollTimeout { get; set; }
        }
    }
}
