using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Identitites.Connections
{
    public sealed class AggregatesConnectionStringIdentity : IdentityBase<AggregatesConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 17; }
        }

        public override string Description
        {
            get { return "Aggs DB connection string"; }
        }
    }
}