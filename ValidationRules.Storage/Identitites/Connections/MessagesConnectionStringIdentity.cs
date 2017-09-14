using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Identitites.Connections
{
    public sealed class MessagesConnectionStringIdentity : IdentityBase<MessagesConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 18;

        public override string Description => "Messages DB connection string";
    }
}