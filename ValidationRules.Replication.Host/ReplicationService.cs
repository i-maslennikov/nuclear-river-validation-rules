using System.ServiceProcess;

using NuClear.Jobs.Schedulers;

namespace NuClear.ValidationRules.Replication.Host
{
    public partial class ReplicationService : ServiceBase
    {
        private readonly ISchedulerManager _schedulerManager;

        public ReplicationService(ISchedulerManager schedulerManager)
        {
            _schedulerManager = schedulerManager;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _schedulerManager.Start();
        }

        protected override void OnStop()
        {
            base.OnStop();
            _schedulerManager.Stop();
        }
    }
}
