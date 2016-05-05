using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.River.Hosting.Common.Settings
{
    public sealed class EnvironmentSettingsAspect : ISettingsAspect, IEnvironmentSettings
    {
        private readonly StringSetting _targetEnvironmentName = ConfigFileSetting.String.Required("TargetEnvironmentName");
        private readonly StringSetting _entryPointName = ConfigFileSetting.String.Required("EntryPointName");

        public string EntryPointName => _entryPointName.Value;

        public string EnvironmentName => _targetEnvironmentName.Value;
    }
}
