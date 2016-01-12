using System.Linq;

using NuClear.Messaging.API.Flows;
using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;

namespace NuClear.Replication.EntryPoint.Factories.Messaging.Receiver
{
    using ValidationRules = NuClear.ValidationRules.OperationsProcessing.Identities.Flows;
    using CustomerIntelligence = NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;

    public sealed class ServiceBusReceiverResolveStrategy : MessageFlowReceiverResolveStrategyBase
    {
        private static readonly IMessageFlow[] Flows = {
                ValidationRules::ImportFactsFromErmFlow.Instance,
                CustomerIntelligence::ImportFactsFromErmFlow.Instance
            };

        public ServiceBusReceiverResolveStrategy(IMetadataProvider metadataProvider)
            : base(
                metadataProvider,
                typeof(ServiceBusOperationsReceiverTelemetryDecorator),
                metadata => metadata.IsPerformedOperationsPrimarySource() && Flows.Contains(metadata.MessageFlow))
        {
        }
    }
}
