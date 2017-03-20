using System;
using System.Globalization;

using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.ValidationRules.Replication.Settings;

namespace NuClear.ValidationRules.Replication.Host.Settings
{
    public sealed class ArchiveVersionsSettings : ISettingsAspect, IArchiveVersionsSettings
    {
        private readonly StringSetting _archiveVersionsInterval = ConfigFileSetting.String.Required("ArchiveVersionsInterval");

        public TimeSpan ArchiveVersionsInterval => TimeSpan.Parse(_archiveVersionsInterval.Value, CultureInfo.InvariantCulture);
    }
}
