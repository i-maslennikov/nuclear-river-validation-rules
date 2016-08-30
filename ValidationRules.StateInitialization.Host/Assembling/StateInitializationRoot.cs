using NuClear.Assembling.Zones;
using NuClear.Replication.Core.Assembling;
using NuClear.ValidationRules.Replication.Assembling;

namespace NuClear.ValidationRules.StateInitialization.Host.Assembling
{
    public sealed class StateInitializationRoot
    {
        public static CompositionRoot Instance => CompositionRoot.Config
                                                                 .RequireZone<ReplicationZone>()
                                                                     .UseAnchor<ReplicationAssembly>()
                                                                     .UseAnchor<ValidationRulesReplicationAssembly>();
    }
}