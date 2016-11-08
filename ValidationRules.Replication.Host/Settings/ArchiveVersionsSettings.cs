using System;
using System.Globalization;

using NuClear.Settings;
using NuClear.Settings.API;
using NuClear.ValidationRules.Replication.Settings;

namespace NuClear.ValidationRules.Replication.Host.Settings
{
    public sealed class ArchiveVersionsSettings : ISettingsAspect, IArchiveVersionsSettings
    {
        private readonly StringSetting _archiveVersionPeriod = ConfigFileSetting.String.Required("ArchiveVersionPeriod");

        public TimeSpan ArchiveVersionPeriod => TimeSpan.Parse(_archiveVersionPeriod.Value, CultureInfo.InvariantCulture);
    }
}