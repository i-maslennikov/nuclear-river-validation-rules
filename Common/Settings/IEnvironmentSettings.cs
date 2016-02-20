using NuClear.Settings.API;

namespace NuClear.River.Common.Settings
{
    public interface IEnvironmentSettings : ISettings
    {
        string EnvironmentName { get; }
        string EntryPointName { get; }
    }
}
