using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication.Host.Temp;

using Quartz;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ResultDeliveryJob : TaskServiceJobBase
    {
        private readonly SlackResultDeliveryService _deliveryService;

        public ResultDeliveryJob(ISignInService signInService,
                                 SlackResultDeliveryService deliveryService,
                                 IUserImpersonationService userImpersonationService,
                                 ITracer tracer)
            : base(signInService, userImpersonationService, tracer)
        {
            _deliveryService = deliveryService;
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            _deliveryService.DoIt();
        }
    }
}
