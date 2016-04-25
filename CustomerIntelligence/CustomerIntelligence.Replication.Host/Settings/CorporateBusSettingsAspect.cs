using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.CustomerIntelligence.Replication.Host.Settings
{
    public sealed class CorporateBusSettingsAspect : ISettingsAspect, ICorporateBusMessageReceiverSettings, ICorporateBusMessageSenderSettings
    {
        private readonly StringSetting _integrationApplicationName = ConfigFileSetting.String.Required("IntegrationApplicationName");

        string ICorporateBusMessageReceiverSettings.ConfigurationEndpointName
        {
            get { return "NetTcpBinding_IBrokerApiReceiver"; }
        }

        string ICorporateBusMessageSenderSettings.ConfigurationEndpointName
        {
            get { return "NetTcpBinding_IBrokerApiSender"; }
        }

        public string IntegrationApplicationName
        {
            get { return _integrationApplicationName.Value; }
        }
    }
}