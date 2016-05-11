using System;
using System.Globalization;

using NuClear.Jobs.Settings;
using NuClear.Settings;
using NuClear.Settings.API;

namespace NuClear.CustomerIntelligence.Replication.Host.Settings
{
    public sealed class QuartzSettingsAspect : ISettingsAspect, IPersistentStoreSettings, ITaskServiceProcessingSettings
    {
        private readonly string _jobStoreConnectionString;
        private readonly IntSetting _maxWorkingThreads = ConfigFileSetting.Int.Required("MaxWorkingThreads");
        private readonly EnumSetting<JobStoreType> _jobStoreType = ConfigFileSetting.Enum.Required<JobStoreType>("JobStoreType");
        private readonly StringSetting _schedulerName = ConfigFileSetting.String.Required("SchedulerName");
        private readonly StringSetting _misfireThreshold = ConfigFileSetting.String.Required("MisfireThreshold");

        public QuartzSettingsAspect(string jobStoreConnectionString)
        {
            _jobStoreConnectionString = jobStoreConnectionString;
        }

        string IPersistentStoreSettings.ConnectionString
        {
            get { return _jobStoreConnectionString; }
        }

        string IPersistentStoreSettings.TablePrefix
        {
            get { return "Quartz."; }
        }

        int ITaskServiceProcessingSettings.MaxWorkingThreads
        {
            get { return _maxWorkingThreads.Value; }
        }

        JobStoreType ITaskServiceProcessingSettings.JobStoreType
        {
            get { return _jobStoreType.Value; }
        }

        string ITaskServiceProcessingSettings.SchedulerName
        {
            get { return _schedulerName.Value; }
        }

        TimeSpan ITaskServiceProcessingSettings.MisfireThreshold
        {
            get { return TimeSpan.Parse(_misfireThreshold.Value, CultureInfo.InvariantCulture); }
        }
    }
}
