using NuClear.Assembling.Zones;
using NuClear.Jobs;
using NuClear.Replication.Core.Assembling;

namespace NuClear.ValidationRules.Replication.Host.DI
{
    public class ReplicationHostAssembly : IZoneAssembly<ReplicationZone>,
                                           IZoneAnchor<ReplicationZone>,
                                           IContainsType<ITaskServiceJob>
    {
    }
}