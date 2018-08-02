using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;
using NuClear.ValidationRules.StateInitialization.Host;
using NuClear.ValidationRules.Storage;
using NuClear.ValidationRules.Storage.Connections;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests
{
    public interface IKey
    {
        ReplicateInBulkCommand Command { get; }
    }

    public sealed class Facts : IKey
    {
        public ReplicateInBulkCommand Command =>
            new ReplicateInBulkCommand(new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                                       new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                                       DbManagementMode.None);
    }

    public sealed class Aggregates : IKey
    {
        public ReplicateInBulkCommand Command =>
            new ReplicateInBulkCommand(new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                                       new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates),
                                       DbManagementMode.None);
    }

    public sealed class Messages : IKey
    {
        public ReplicateInBulkCommand Command =>
            new ReplicateInBulkCommand(new StorageDescriptor(AggregatesConnectionStringIdentity.Instance, Schema.Aggregates),
                                       new StorageDescriptor(MessagesConnectionStringIdentity.Instance, Schema.Messages),
                                       DbManagementMode.None);
    }
}
