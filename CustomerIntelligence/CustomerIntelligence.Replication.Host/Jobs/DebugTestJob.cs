using System;
using System.Threading.Tasks;

using NuClear.Jobs;
using NuClear.Security.API;
using NuClear.Tracing.API;

using Quartz;

namespace NuClear.CustomerIntelligence.Replication.Host.Jobs
{
    public sealed class DebugTestJob : TaskServiceJobBase
    {
        public DebugTestJob(ITracer tracer, ISignInService signInService, IUserImpersonationService userImpersonationService)
            : base(signInService, userImpersonationService, tracer)
        {
        }

        protected override void ExecuteInternal(IJobExecutionContext context)
        {
            Console.WriteLine("Enter DebugTestJob, time: {0}", DateTime.UtcNow);
            Task.Delay(5000);
            Console.WriteLine("Exit DebugTestJob, time: {0}", DateTime.UtcNow);
        }
    }
}