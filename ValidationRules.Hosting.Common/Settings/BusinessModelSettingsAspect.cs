using NuClear.Settings;
using NuClear.Settings.API;

namespace ValidationRules.Hosting.Common.Settings
{
    public sealed class BusinessModelSettingsAspect : ISettingsAspect, ISettings, IBusinessModelSettings
    {
        public string BusinessModel { get; } = ConfigFileSetting.String.Required("BusinessModel").Value;
    }
}