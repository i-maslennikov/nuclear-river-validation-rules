using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Connections
{
    public class ErmConnectionStringIdentity : IdentityBase<ErmConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 15;

        public override string Description => "Erm DB connnection string";
    }
}