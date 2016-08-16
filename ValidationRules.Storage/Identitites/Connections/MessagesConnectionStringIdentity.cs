using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.ValidationRules.Storage.Identitites.Connections
{
    public sealed class MessagesConnectionStringIdentity : IdentityBase<MessagesConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id
        {
            get { return 18; }
        }

        public override string Description
        {
            get { return "Messages DB connection string"; }
        }
    }
}