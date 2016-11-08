using System;

using NuClear.Settings.API;

namespace NuClear.ValidationRules.Replication.Settings
{
    public interface IArchiveVersionsSettings : ISettings
    {
        TimeSpan ArchiveVersionPeriod { get; }
    }
}
