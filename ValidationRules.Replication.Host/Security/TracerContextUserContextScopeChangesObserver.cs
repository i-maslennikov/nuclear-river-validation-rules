using NuClear.Security.API.Context;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.Replication.Host.Security
{
    internal sealed class TracerContextUserContextScopeChangesObserver : IUserContextScopeChangesObserver
    {
        private readonly ITracer _tracer;

        public TracerContextUserContextScopeChangesObserver(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void ScopeChanged(IUserContextScope currentScope)
        {
            SetIdentityNameFromScope(currentScope);
        }

        public void ValueAttached(IUserContextScope currentScope, IUserContextKey key, object value)
        {
            SetIdentityNameFromScope(currentScope);
        }

        private void SetIdentityNameFromScope(IUserContextScope currentScope)
        {
            _tracer.Context.Push(TracerContextKeys.UserAccount, "Not available");
        }
    }
}