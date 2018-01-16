using System.Collections.Generic;
using System.ServiceProcess;

using NuClear.Jobs.Schedulers;

namespace NuClear.ValidationRules.Replication.Host
{
    public partial class ReplicationService : ServiceBase
    {
        private readonly IReadOnlyCollection<ISchedulerManager> _schedulerManagers;

        public ReplicationService(IReadOnlyCollection<ISchedulerManager> schedulerManagers)
        {
            _schedulerManagers = schedulerManagers;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);

            foreach (var schedulerManager in _schedulerManagers)
            {
                schedulerManager.Start();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();

            foreach (var schedulerManager in _schedulerManagers)
            {
                schedulerManager.Stop();
            }
        }
    }
}
