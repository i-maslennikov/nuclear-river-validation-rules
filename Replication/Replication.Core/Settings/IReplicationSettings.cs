using NuClear.Settings.API;

namespace NuClear.Replication.Core.Settings
{
    public interface IReplicationSettings : ISettings
    {
        int ReplicationBatchSize { get; }
    }
}