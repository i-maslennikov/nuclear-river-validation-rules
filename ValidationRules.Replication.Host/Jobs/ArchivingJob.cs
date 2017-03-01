using System;
using System.Transactions;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Telemetry.Probing;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication.Messages;
using NuClear.ValidationRules.Replication.Settings;

using Quartz;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public class ArchivingJob : TaskServiceJobBase
    {
        private readonly ArchiveVersionsService _archiveVersionsService;
        private readonly IArchiveVersionsSettings _settings;
        private readonly TransactionOptions _transactionOptions;

        public ArchivingJob(
            ISignInService signInService,
            IUserImpersonationService userImpersonationService,
            ArchiveVersionsService archiveVersionsService,
            IArchiveVersionsSettings settings,
            ITracer tracer)
            : base(signInService, userImpersonationService, tracer)
        {
            _archiveVersionsService = archiveVersionsService;
            _settings = settings;
            _transactionOptions = new TransactionOptions { IsolationLevel = IsolationLevel.Snapshot, Timeout = TimeSpan.Zero };
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            var archiveDate = DateTime.UtcNow - _settings.ArchiveVersionsInterval;
            using (Probe.Create("Archive"))
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, _transactionOptions))
            {
                _archiveVersionsService.Execute(archiveDate);
                transaction.Complete();
            }
        }
    }
}