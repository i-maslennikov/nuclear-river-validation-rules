using NuClear.Model.Common;
using NuClear.Storage.API.ConnectionStrings;

namespace NuClear.River.Hosting.Common.Identities.Connections
{
    public class ServiceBusConnectionStringIdentity : IdentityBase<ServiceBusConnectionStringIdentity>, IConnectionStringIdentity
    {
        public override int Id => 5;

        public override string Description => "MS Service Bus connection string identity";
    }
}