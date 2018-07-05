using NuClear.Settings.API;

namespace ValidationRules.Hosting.Common.Settings
{
    public interface IBusinessModelSettings : ISettings
    {
        string BusinessModel { get; }
    }
}
