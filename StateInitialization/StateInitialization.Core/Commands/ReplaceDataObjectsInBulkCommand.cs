using NuClear.Replication.Core;
using NuClear.StateInitialization.Core.Storage;

namespace NuClear.StateInitialization.Core.Commands
{
    public sealed class ReplaceDataObjectsInBulkCommand : ICommand
    {
        public ReplaceDataObjectsInBulkCommand(StorageDescriptor sourceStorageDescriptor, StorageDescriptor targetStorageDescriptor)
        {
            SourceStorageDescriptor = sourceStorageDescriptor;
            TargetStorageDescriptor = targetStorageDescriptor;
        }

        public StorageDescriptor SourceStorageDescriptor { get; }
        public StorageDescriptor TargetStorageDescriptor { get; }
    }
}