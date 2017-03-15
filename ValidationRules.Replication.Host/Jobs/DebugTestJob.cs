using System;
using System.Threading.Tasks;

using NuClear.Jobs;
using NuClear.Security.API.Context;
using NuClear.Security.API.Auth;
using NuClear.Tracing.API;

using Quartz;

namespace NuClear.ValidationRules.Replication.Host.Jobs
{
    public sealed class DebugTestJob : TaskServiceJobBase
    {
        public DebugTestJob(IUserContextManager userContextManager,
                            IUserAuthenticationService userAuthenticationService,
                            IUserAuthorizationService userAuthorizationService,
                            ITracer tracer)
            : base(userContextManager, userAuthenticationService, userAuthorizationService, tracer)
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