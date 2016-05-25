using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public class StateIncrementedEvent : IEvent
    {
        public StateIncrementedEvent(IReadOnlyCollection<Guid> includedTokens)
        {
            IncludedTokens = includedTokens;
        }

        public IReadOnlyCollection<Guid> IncludedTokens { get; }
    }
}