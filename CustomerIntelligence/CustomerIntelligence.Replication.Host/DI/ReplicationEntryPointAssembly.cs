using NuClear.Assembling.Zones;
using NuClear.Jobs;

namespace NuClear.CustomerIntelligence.Replication.Host.DI
{
    public class ReplicationEntryPointAssembly : IZoneAssembly<ReplicationZone>,
                                                 IZoneAnchor<ReplicationZone>,
                                                 IContainsType<ITaskServiceJob>
    {
    }
}