using NuClear.Assembling.Zones;
using NuClear.CustomerIntelligence.Replication.Assembling;
using NuClear.Replication.Core.Assembling;

namespace NuClear.CustomerIntelligence.Replication.Host.DI
{
    internal static class ReplicationRoot
    {
        public static CompositionRoot Instance => CompositionRoot.Config
                                                                 .RequireZone<ReplicationZone>()
                                                                     .UseAnchor<ReplicationAssembly>()
                                                                     .UseAnchor<CustomerIntelligenceReplicationAssembly>()
                                                                     .UseAnchor<ReplicationHostAssembly>();
    }
}