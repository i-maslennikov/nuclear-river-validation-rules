using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Slack
{
    public sealed class DebugTransportDecorator : ITransportDecorator
    {
        public IReadOnlyCollection<string> GetSubscribedUsers()
        {
            return new[] { "a.rechkalov" };
        }

        public void SendMessage(string user, IReadOnlyCollection<LocalizedMessage> messages)
        {
            Debug.WriteLine($"{user}");
            foreach (var message in messages.GroupBy(x => x.Header, x => x))
            {
                Debug.WriteLine($"\t{message.Key}");
                foreach (var item in message.OrderBy(x => x.Result))
                {
                    Debug.WriteLine($"\t\t{GetIcon(item.Result)} {item.Message}");
                }
            }
        }

        private string GetIcon(Result result)
        {
            switch (result)
            {
                case Result.Info:
                    return ":information_source:";
                case Result.Warning:
                    return ":warning:";
                case Result.Error:
                    return ":exclamation:";
                default:
                    return string.Empty;
            }
        }
    }
}