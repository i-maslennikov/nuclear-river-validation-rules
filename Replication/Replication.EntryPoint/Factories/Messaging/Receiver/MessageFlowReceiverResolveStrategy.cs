using System;
using System.Linq;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.River.Common.Metadata.Features;

namespace NuClear.Replication.EntryPoint.Factories.Messaging.Receiver
{
    public class MessageFlowReceiverResolveStrategy : IMessageFlowReceiverResolveStrategy
    {
        public bool TryGetAppropriateReceiver(MessageFlowMetadata messageFlowMetadata, out Type resolvedFlowReceiverType)
        {
            var receiverTypeFeature = messageFlowMetadata.Features.OfType<ReceiverTypeFeature>().FirstOrDefault();

            if (receiverTypeFeature != null)
            {
                resolvedFlowReceiverType = receiverTypeFeature.Type;
                return true;
            }

            resolvedFlowReceiverType = null;
            return false;
        }
    }
}