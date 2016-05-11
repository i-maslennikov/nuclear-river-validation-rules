using System;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.DI.Factories.Unity.Transformers.Resolvers;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;

namespace NuClear.ValidationRules.Replication.Host.Factories.Messaging.Transformer
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
