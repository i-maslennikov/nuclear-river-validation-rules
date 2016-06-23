using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.ServiceBus.API;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus.Factories
{
    public interface IServiceBusSettingsFactory
    {
        IServiceBusMessageReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow);
        IServiceBusMessageSenderSettings CreateSenderSettings(IMessageFlow messageFlow);
    }
}