using System.Collections.Generic;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery
{
    public interface ITransportDecorator
    {
        IReadOnlyCollection<string> GetSubscribedUsers();
        void SendMessage(string user, IReadOnlyCollection<LocalizedMessage> messages);
    }
}