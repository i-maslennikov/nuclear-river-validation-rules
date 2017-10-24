using NuClear.Messaging.API.Flows;
using NuClear.Messaging.Transports.Kafka;

namespace ValidationRules.Hosting.Common
{
    public interface IKafkaSettingsFactory
    {
        IKafkaMessageFlowReceiverSettings CreateReceiverSettings(IMessageFlow messageFlow);
        IKafkaMessageFlowInfoSettings CreateInfoSettings(IMessageFlow messageFlow);
    }
}