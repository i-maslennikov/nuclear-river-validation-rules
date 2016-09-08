using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests.Identitites.Connections
{
    internal sealed class MessagesTestConnectionStringIdentity : IdentityBase<MessagesTestConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 65148461; }
        }

        public override string Description
        {
            get { return "Messages DB connection string identity (state initialization testing scope)"; }
        }
    }
}