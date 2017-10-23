using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;

namespace NuClear.ValidationRules.OperationsProcessing.Transports.Kafka
{
    public interface IAmsSettingsFactory
    {
        IKafkaMessageFlowReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow);
    }
}