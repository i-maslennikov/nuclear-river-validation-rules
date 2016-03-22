using NuClear.River.Common.Settings;
using NuClear.Settings.API;

namespace NuClear.Querying.Http
{
    public abstract class SettingsContainer : SettingsContainerBase
    {
        protected SettingsContainer()
        {
            Aspects.Use<EnvironmentSettingsAspect>();
        }
    }
}