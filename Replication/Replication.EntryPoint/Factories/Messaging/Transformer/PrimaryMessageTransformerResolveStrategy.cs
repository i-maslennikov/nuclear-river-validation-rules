using System;
using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.DI.Factories.Unity.Transformers.Resolvers;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;

namespace NuClear.Replication.EntryPoint.Factories.Messaging.Transformer
{
    using ValidationRules = NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
    using CustomerIntelligence = NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;

    public sealed class PrimaryMessageTransformerResolveStrategy : IMessageTransformerResolveStrategy
    {
        private static readonly IMessageFlow[] Flows = {
                ValidationRules::ImportFactsFromErmFlow.Instance,
                CustomerIntelligence::ImportFactsFromErmFlow.Instance
            };

        public bool TryGetAppropriateTransformer(MessageFlowMetadata messageFlowMetadata, out Type resolvedFlowReceiverType)
        {
            if (messageFlowMetadata.IsPerformedOperationsPrimarySource() && Flows.Contains(messageFlowMetadata.MessageFlow))
            {
                resolvedFlowReceiverType = typeof(BinaryEntireBrokeredMessage2TrackedUseCaseTransformer);
                return true;
            }

            resolvedFlowReceiverType = null;
            return false;
        }
    }
}
