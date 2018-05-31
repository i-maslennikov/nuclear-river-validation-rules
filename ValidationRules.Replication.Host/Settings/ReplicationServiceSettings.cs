using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

using Jobs.RemoteControl.Settings;

using NuClear.IdentityService.Client.Settings;
using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Replication.Core.Settings;
using NuClear.River.Hosting.Common.Identities.Connections;
using NuClear.River.Hosting.Common.Settings;
using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Telemetry.Logstash;
using NuClear.ValidationRules.Storage.Connections;

using Quartz.Impl;

namespace NuClear.ValidationRules.Replication.Host.Settings
{
    public sealed class ReplicationServiceSettings : SettingsContainerBase, IReplicationSettings, ISqlStoreSettingsAspect
    {
        private readonly IntSetting _replicationBatchSize = ConfigFileSetting.Int.Required("ReplicationBatchSize");
        private readonly IntSetting _sqlCommandTimeout = ConfigFileSetting.Int.Required("SqlCommandTimeout");

        public ReplicationServiceSettings()
        {
            var connectionStringSettings = new ConnectionStringSettingsAspect(
                new Dictionary<IConnectionStringIdentity, string>
                {
                    [ErmConnectionStringIdentity.Instance] = GetConnectionString("Erm"),
                    [AmsConnectionStringIdentity.Instance] = GetConnectionString("Ams"),
                    [RulesetConnectionStringIdentity.Instance] = GetConnectionString("Ams"),
                    [FactsConnectionStringIdentity.Instance] = GetConnectionString("Facts"),
                    [AggregatesConnectionStringIdentity.Instance] = GetConnectionString("Aggregates"),
                    [MessagesConnectionStringIdentity.Instance] = GetConnectionString("Messages"),
                    [ServiceBusConnectionStringIdentity.Instance] = GetConnectionString("ServiceBus"),
                    [InfrastructureConnectionStringIdentity.Instance] = GetConnectionString("Infrastructure"),
                    [LoggingConnectionStringIdentity.Instance] = GetConnectionString("Logging")
                });

            var quartzProperties = (NameValueCollection)ConfigurationManager.GetSection(StdSchedulerFactory.ConfigurationSectionName);

            Aspects.Use(connectionStringSettings)
                   .Use<ServiceBusMessageLockRenewalSettings>()
                   .Use<EnvironmentSettingsAspect>()
                   .Use(new QuartzSettingsAspect(connectionStringSettings.GetConnectionString(InfrastructureConnectionStringIdentity.Instance)))
                   .Use(new ServiceBusReceiverSettingsAspect(connectionStringSettings.GetConnectionString(ServiceBusConnectionStringIdentity.Instance)))
                   .Use<ArchiveVersionsSettings>()
                   .Use<LogstashSettingsAspect>()
                   .Use<IdentityServiceClientSettingsAspect>()
                   .Use(new TaskServiceRemoteControlSettings(quartzProperties));
        }

        public int ReplicationBatchSize
        {
            get { return _replicationBatchSize.Value; }
        }

        public int SqlCommandTimeout
        {
            get { return _sqlCommandTimeout.Value; }
        }

        private static string GetConnectionString(string connnectionStringKey)
        {
            return ConfigurationManager.ConnectionStrings[connnectionStringKey].ConnectionString;
        }
    }
}