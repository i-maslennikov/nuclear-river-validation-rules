using NuClear.Assembling.Zones;
using NuClear.Replication.Core.Assembling;

namespace NuClear.ValidationRules.Replication.Assembling
{
    public sealed class ValidationRulesReplicationAssembly : IZoneAssembly<ReplicationZone>,
                                                                  IZoneAnchor<ReplicationZone>
    {
    }
}