using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public class AggregatesStateIncrementedEvent : IEvent
    {
        public AggregatesStateIncrementedEvent(IReadOnlyCollection<Guid> includedTokens)
        {
            IncludedTokens = includedTokens;
        }

        public IReadOnlyCollection<Guid> IncludedTokens { get; }
    }
}