using NuClear.Settings.API;

namespace NuClear.Replication.Core.Settings
{
    public interface ISqlStoreSettingsAspect : ISettings
    {
        int SqlCommandTimeout { get; }
    }
}