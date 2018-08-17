using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Connections
{
    public sealed class AggregatesConnectionStringIdentity : IdentityBase<AggregatesConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 17;

        public override string Description => "Aggregates DB connection string";
    }
}