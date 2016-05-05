using NuClear.Settings.API;

namespace NuClear.River.Hosting.Common.Settings
{
    public interface IEnvironmentSettings : ISettings
    {
        string EnvironmentName { get; }
        string EntryPointName { get; }
    }
}
