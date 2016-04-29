using NuClear.Assembling.Zones;
using NuClear.CustomerIntelligence.Replication.Assembling;
using NuClear.Replication.Core.Assembling;

namespace NuClear.CustomerIntelligence.StateInitialization.Host.Assembling
{
    public sealed class StateInitializationRoot
    {
        public static CompositionRoot Instance => CompositionRoot.Config
                                                                 .RequireZone<ReplicationZone>()
                                                                     .UseAnchor<ReplicationAssembly>()
                                                                     .UseAnchor<CustomerIntelligenceReplicationAssembly>();
    }
}