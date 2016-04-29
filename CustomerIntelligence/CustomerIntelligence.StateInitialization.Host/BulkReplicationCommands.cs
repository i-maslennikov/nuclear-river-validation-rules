using NuClear.CustomerIntelligence.Storage;
using NuClear.CustomerIntelligence.Storage.Identitites.Connections;
using NuClear.StateInitialization.Core.Commands;
using NuClear.StateInitialization.Core.Storage;

namespace NuClear.CustomerIntelligence.StateInitialization.Host
{
    public static class BulkReplicationCommands
    {
        public static readonly ReplaceDataObjectsInBulkCommand ErmToFacts =
            new ReplaceDataObjectsInBulkCommand(
                new StorageDescriptor(ErmConnectionStringIdentity.Instance, Schema.Erm),
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts));

        public static readonly ReplaceDataObjectsInBulkCommand FactsToCi =
            new ReplaceDataObjectsInBulkCommand(
                new StorageDescriptor(FactsConnectionStringIdentity.Instance, Schema.Facts),
                new StorageDescriptor(CustomerIntelligenceConnectionStringIdentity.Instance, Schema.CustomerIntelligence));
    }
}