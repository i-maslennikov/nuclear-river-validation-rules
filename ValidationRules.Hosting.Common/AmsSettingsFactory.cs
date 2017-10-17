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
            _config = JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionString);
        }

        public IKafkaMessageFlowReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow)
        {
            if (AmsFactsFlow.Instance.Equals(messageFlow))
            {
                return new KafkaMessageFlowReceiverSettings
                {
                    ClientId = _environmentSettings.EntryPointName + '-' + _environmentSettings.EnvironmentName,
                    GroupId = messageFlow.Id.ToString() + '-' + _environmentSettings.EnvironmentName,
                    Config = _config,
                    Offset = _offset,
                    Topics = Topics,
                    PollTimeout = PollTimeout
                };
            }

            throw new ArgumentException($"Flow '{messageFlow.Description}' settings for Kafka are undefined");
        }

        private sealed class KafkaMessageFlowReceiverSettings : IKafkaMessageFlowReceiverSettings
        {
            public string ClientId { get; set; }
            public string GroupId { get; set; }
            public Dictionary<string, object> Config { get; set; }
            public Offset Offset { get; set; }

            public IEnumerable<string> Topics { get; set; }
            public TimeSpan PollTimeout { get; set; }
        }
    }
}
