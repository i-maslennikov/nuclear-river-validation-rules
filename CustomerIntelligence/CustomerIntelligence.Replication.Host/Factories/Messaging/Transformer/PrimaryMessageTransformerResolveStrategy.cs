using System;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.DI.Factories.Unity.Transformers.Resolvers;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;

namespace NuClear.CustomerIntelligence.Replication.Host.Factories.Messaging.Transformer
{
    public sealed class PrimaryMessageTransformerResolveStrategy : IMessageTransformerResolveStrategy
    {
        public bool TryGetAppropriateTransformer(MessageFlowMetadata messageFlowMetadata, out Type resolvedFlowReceiverType)
        {
            var messageFlow = messageFlowMetadata.MessageFlow;

            if (messageFlowMetadata.IsPerformedOperationsPrimarySource() && messageFlow.Equals(ImportFactsFromErmFlow.Instance))
            {
                resolvedFlowReceiverType = typeof(BinaryEntireBrokeredMessage2TrackedUseCaseTransformer);
                return true;
            }

            resolvedFlowReceiverType = null;
            return false;
        }
    }
}
