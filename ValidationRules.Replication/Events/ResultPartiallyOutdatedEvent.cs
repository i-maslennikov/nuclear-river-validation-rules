using System.Collections.Generic;

using NuClear.Replication.Core;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.Events
{
    public class ResultPartiallyOutdatedEvent : IEvent
    {
        public ResultPartiallyOutdatedEvent(MessageTypeCode rule, IReadOnlyCollection<long> orderIds)
        {
            Rule = rule;
            OrderIds = orderIds;
        }

        public MessageTypeCode Rule { get; }
        public IReadOnlyCollection<long> OrderIds { get; }
    }
}