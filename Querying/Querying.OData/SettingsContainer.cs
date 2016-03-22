using NuClear.River.Common.Settings;
using NuClear.Settings.API;

namespace NuClear.Querying.OData
{
    public abstract class SettingsContainer : SettingsContainerBase
    {
        protected SettingsContainer()
        {
            Aspects.Use<EnvironmentSettingsAspect>();
        }
    }
}