using NuClear.Assembling.Zones;
using NuClear.Replication.Core.Assembling;
using NuClear.ValidationRules.Replication.Assembling;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests.DI
{
    internal static class StateInitializationTestsRoot
    {
        public static CompositionRoot Instance => CompositionRoot.Config
                                                                 .RequireZone<ReplicationZone>()
                                                                 .UseAnchor<ReplicationAssembly>()
                                                                 .UseAnchor<ValidationRulesReplicationAssembly>();
    }
}