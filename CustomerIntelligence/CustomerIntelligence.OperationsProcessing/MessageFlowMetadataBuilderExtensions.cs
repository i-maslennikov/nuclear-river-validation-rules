using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.Replication.OperationsProcessing.Metadata;

namespace NuClear.CustomerIntelligence.OperationsProcessing
{
    public static class MessageFlowMetadataBuilderExtensions
    {
        // todo: можно вместе с фичей переместить в 2GIS.NuClear.Messaging.API
        public static MessageFlowMetadataBuilder Receiver<T>(this MessageFlowMetadataBuilder builder)
            where T : IMessageReceiver
        {
            return builder.WithFeatures(new ReceiverTypeFeature(typeof(T)));
        }
    }
}