
using NuClear.Messaging.API.Flows;
using NuClear.Replication.Core;
using NuClear.StateInitialization.Core.Commands;

namespace NuClear.ValidationRules.StateInitialization.Host.Kafka
{
    internal sealed class KafkaReplicationCommand : ICommand
    {
        public KafkaReplicationCommand(IMessageFlow messageFlow, ReplicateInBulkCommand replicateInBulkCommand)
        {
            MessageFlow = messageFlow;
            ReplicateInBulkCommand = replicateInBulkCommand;
        }

        public IMessageFlow MessageFlow { get; }
        public ReplicateInBulkCommand ReplicateInBulkCommand { get; }
    }
}
