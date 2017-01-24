using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class FactsStateIncrementedEvent : IEvent
    {
        public FactsStateIncrementedEvent(IReadOnlyCollection<Guid> includedTokens)
        {
            IncludedTokens = includedTokens;
        }

        public IReadOnlyCollection<Guid> IncludedTokens { get; }
    }
}