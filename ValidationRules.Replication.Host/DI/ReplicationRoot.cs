using NuClear.Assembling.Zones;
using NuClear.ValidationRules.Replication.Assembling;
using NuClear.Replication.Core.Assembling;

namespace NuClear.ValidationRules.Replication.Host.DI
{
    internal static class ReplicationRoot
    {
        public static CompositionRoot Instance => CompositionRoot.Config
                                                                 .RequireZone<ReplicationZone>()
                                                                     .UseAnchor<ReplicationAssembly>()
                                                                     .UseAnchor<ValidationRulesReplicationAssembly>()
                                                                     .UseAnchor<ReplicationHostAssembly>();
    }
}