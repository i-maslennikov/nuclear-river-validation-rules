using NuClear.Assembling.Zones;

namespace NuClear.CustomerIntelligence.Replication.Host.DI
{
    internal static class ReplicationRoot
    {
        public static CompositionRoot Instance => CompositionRoot.Config
                                                                 .RequireZone<ReplicationZone>().UseAnchor<ReplicationEntryPointAssembly>();
    }
}