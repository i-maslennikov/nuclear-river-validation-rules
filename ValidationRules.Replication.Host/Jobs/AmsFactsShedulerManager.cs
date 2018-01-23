using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

using NuClear.Jobs.Schedulers;

using Quartz;
using Quartz.Impl;
using Quartz.Plugin.Xml;
using Quartz.Simpl;
using Quartz.Spi;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    // Kafka сама занимается балансировкой, поэтому smart scheduler, опирающийся на базу данных, не подойдёт
    // нужен тупой RAM-based sheduler (или же совсем без quartz обходиться)
    // чтобы все jobs на всех нодах постоянно работали
    internal sealed class AmsFactsShedulerManager : ISchedulerManager
    {
        private const string SchedulerName = "AmsFactsSheduler";

        // хотел назвать quartz.ams.config, но стандартный Scheduler
        // процессит все файлы quartz*.config, поэтому пришлось изголяться
        // чтобы этот файл не попал под массовый процессинг
        private const string ConfigName = "quartz.ams_config";

        private readonly IJobFactory _jobFactory;

        public AmsFactsShedulerManager(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
        }

        public void Start()
        {
            var instanceId = new SimpleInstanceIdGenerator().GenerateInstanceId();

            var threadPool = new SimpleThreadPool(1, ThreadPriority.Normal) { InstanceName = SchedulerName };
            threadPool.Initialize();

            var jobstore = new RAMJobStore
            {
                InstanceName = SchedulerName,
                InstanceId = instanceId,
            };

            var baseUri = new Uri(Assembly.GetExecutingAssembly().GetName().EscapedCodeBase);
            // ReSharper disable once AssignNullToNotNullAttribute
            var fileName = Path.Combine(Path.GetDirectoryName(baseUri.LocalPath), ConfigName);

            var jobInitializationPlugin = new ConfigFileProcessorPlugin
            {
                FileNames = fileName,
                ScanInterval = QuartzConfigFileScanInterval.DisableScanning
            };

            DirectSchedulerFactory.Instance.CreateScheduler(
                SchedulerName,
                instanceId,
                threadPool,
                jobstore,
                new Dictionary<string, ISchedulerPlugin>
                {
                    { SchedulerName, jobInitializationPlugin }
                },
                TimeSpan.Zero,
                TimeSpan.Zero);

            var scheduler = DirectSchedulerFactory.Instance.GetScheduler(SchedulerName);
            scheduler.JobFactory = _jobFactory;

            scheduler.Start();
        }

        public void Stop()
        {
            var scheduler = DirectSchedulerFactory.Instance.GetScheduler(SchedulerName);
            scheduler.Shutdown(true);
        }

        // copy\paste from 'jobs' repo
        private sealed class ConfigFileProcessorPlugin : XMLSchedulingDataProcessorPlugin
        {
            public override void Initialize(string pluginName, IScheduler sched)
            {
                sched.Clear();
                base.Initialize(pluginName, sched);
            }
        }
    }
}