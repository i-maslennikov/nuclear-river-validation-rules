using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests.Identitites.Connections
{
    internal sealed class ErmTestConnectionStringIdentity : IdentityBase<ErmTestConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 21; }
        }

        public override string Description
        {
            get { return "Erm DB connnection string (state initialization testing scope)"; }
        }
    }
}