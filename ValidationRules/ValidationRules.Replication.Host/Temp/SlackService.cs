using System.Collections.Generic;
using System.Diagnostics;

namespace NuClear.ValidationRules.Replication.Host.Temp
{
    public sealed class SlackDecorator
    {
        public IReadOnlyCollection<string> GetUsers()
        {
            return new[] { "a.rechkalov" };
        }

        public void SendMessage(string user, string message)
        {
            Debug.WriteLine($"{user}: {message}");
        }
    }
}