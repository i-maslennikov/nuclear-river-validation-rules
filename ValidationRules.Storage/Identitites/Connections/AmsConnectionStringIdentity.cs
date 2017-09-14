using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Identitites.Connections
{
    public sealed class AmsConnectionStringIdentity : IdentityBase<AmsConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 19;

        public override string Description => "AMS connection string identity";
    }
}