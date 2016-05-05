using NuClear.Settings.API;

namespace NuClear.River.Hosting.Common.Settings
{
    public abstract class SettingsContainer : SettingsContainerBase
    {
        protected SettingsContainer()
        {
            Aspects.Use<EnvironmentSettingsAspect>();
        }
    }
}