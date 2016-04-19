using System.Collections.Generic;
using System.Configuration;

using NuClear.IdentityService.Client.Settings;
using NuClear.OperationsLogging.Transports.ServiceBus;
using NuClear.Replication.Core.API.Settings;
using NuClear.River.Common.Identities.Connections;
using NuClear.River.Common.Settings;
using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.Storage.API.ConnectionStrings;
using NuClear.Telemetry.Logstash;
using NuClear.ValidationRules.Storage.Identitites.Connections;

namespace NuClear.Replication.EntryPoint.Settings
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
                    {
                        ErmConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings["Erm"].ConnectionString
                    },
                    {
                        FactsConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings["Facts"].ConnectionString
                    },
                    {
                        AggregatesConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings["Aggregates"].ConnectionString
                    },
                    {
                        TransportConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings["Transport"].ConnectionString
                    },
                    {
                        ServiceBusConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings["ServiceBus"].ConnectionString
                    },
                    {
                        InfrastructureConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings["Infrastructure"].ConnectionString
                    },
                    {
                        LoggingConnectionStringIdentity.Instance,
                        ConfigurationManager.ConnectionStrings["Logging"].ConnectionString
                    }
                });

            Aspects.Use(connectionStringSettings)
                   .Use<ServiceBusMessageLockRenewalSettings>()
                   .Use<EnvironmentSettingsAspect>()
                   .Use(new QuartzSettingsAspect(connectionStringSettings.GetConnectionString(InfrastructureConnectionStringIdentity.Instance)))
                   .Use(new ServiceBusReceiverSettingsAspect(connectionStringSettings.GetConnectionString(ServiceBusConnectionStringIdentity.Instance)))
                   .Use<CorporateBusSettingsAspect>()
                   .Use<LogstashSettingsAspect>()
                   .Use<IdentityServiceClientSettingsAspect>();
        }

        public int ReplicationBatchSize
        {
            get { return _replicationBatchSize.Value; }
        }

        public int SqlCommandTimeout
        {
            get { return _sqlCommandTimeout.Value; }
        }
    }
}