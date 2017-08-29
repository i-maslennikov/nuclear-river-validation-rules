using System;
using System.Collections.Generic;
using System.Globalization;

using Confluent.Kafka;
using Newtonsoft.Json;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow;
using NuClear.ValidationRules.OperationsProcessing.Transports.Kafka;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.ValidationRules.Replication.Host.Factories
{
    public sealed class AmsSettingsFactory : IAmsSettingsFactory
    {
        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IEnvironmentSettings _environmentSettings;

        public AmsSettingsFactory(IConnectionStringSettings connectionStringSettings, IEnvironmentSettings environmentSettings)
        {
            _connectionStringSettings = connectionStringSettings;
            _environmentSettings = environmentSettings;
        }

        public IKafkaMessageFlowReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow)
        {
            if (AmsFactsFlow.Instance.Equals(messageFlow))
                return new ReceiverSettings(_connectionStringSettings)
                    {
                        ClientId = _environmentSettings.EntryPointName + '-' + _environmentSettings.EnvironmentName,
                        GroupId = messageFlow.Id.ToString() + '-' + _environmentSettings.EnvironmentName,
                    };

            throw new ArgumentException($"Flow '{messageFlow.Description}' settings for Kafka are undefined");
        }

        private sealed class ReceiverSettings : IKafkaMessageFlowReceiverSettings
        {
            private readonly StringSetting _amsFactsTopics = ConfigFileSetting.String.Optional("AmsFactsTopics", "ams_okapi_vr_integration.am.validity");
            private readonly StringSetting _pollTimeout = ConfigFileSetting.String.Optional("AmsPollTimeout", "00:00:05");

            public ReceiverSettings(IConnectionStringSettings connectionStringSettings)
            {
                var connectionString = connectionStringSettings.GetConnectionString(AmsConnectionStringIdentity.Instance);
                Config = JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionString);
            }

            public string ClientId { get; set; }
            public string GroupId { get; set; }
            public Dictionary<string, object> Config { get; }

            public IEnumerable<string> Topics => _amsFactsTopics.Value.Split(',');
            public TimeSpan PollTimeout => TimeSpan.Parse(_pollTimeout.Value, CultureInfo.InvariantCulture);

            public Offset Offset { get; } = Offset.Invalid;
        }
    }
}
