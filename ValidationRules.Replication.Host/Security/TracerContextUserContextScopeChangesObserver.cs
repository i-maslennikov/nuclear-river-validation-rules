using NuClear.Security.API.Context;
using NuClear.Tracing.API;

namespace NuClear.ValidationRules.Replication.Host.Security
{
    internal sealed class TracerContextUserContextScopeChangesObserver : IUserContextScopeChangesObserver
    {
        private readonly ITracerContextManager _tracerContextManager;

        public TracerContextUserContextScopeChangesObserver(ITracerContextManager tracerContextManager)
        {
            _tracerContextManager = tracerContextManager;
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
            _tracerContextManager[TracerContextKeys.Required.UserAccount] = "Not available";
        }
    }
}