using System.Security.Principal;

using NuClear.Security.API;
using NuClear.Security.API.Auth;

namespace NuClear.ValidationRules.Replication.Host.Security
{
    internal sealed class UserAuthorizationService : IUserAuthorizationService
    {
        public bool TryAuthorize(IIdentity identity, out IUserPrincipal principal)
        {
            principal = UserPrincipal.Instance;
            return true;
        }

        private class UserPrincipal : IUserPrincipal
        {
            public static readonly UserPrincipal Instance = new UserPrincipal();

            public bool HasClaim(string claim) => false;
            public IUserIdentity Identity => UserIdentity.Instance;
        }

        private class UserIdentity : IUserIdentity
        {
            public static readonly UserIdentity Instance = new UserIdentity();

            public object Code => null;
            public string Name => null;
        }
    }
}