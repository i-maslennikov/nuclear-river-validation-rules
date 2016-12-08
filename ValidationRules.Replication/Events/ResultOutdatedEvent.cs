using NuClear.Replication.Core;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Events
{
    public class ResultOutdatedEvent : IEvent
    {
        public ResultOutdatedEvent(MessageTypeCode rule)
        {
            Rule = rule;
        }

        public MessageTypeCode Rule { get; }
    }
}