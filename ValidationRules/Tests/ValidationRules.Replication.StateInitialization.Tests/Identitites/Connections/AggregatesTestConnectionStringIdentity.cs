using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Replication.StateInitialization.Tests.Identitites.Connections
{
    internal sealed class AggregatesTestConnectionStringIdentity : IdentityBase<AggregatesTestConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 22; }
        }

        public override string Description
        {
            get { return "Aggregates DB connection string identity (state initialization testing scope)"; }
        }
    }
}