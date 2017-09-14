using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class ErmStateIncrementedEvent : IEvent
    {
        public ErmStateIncrementedEvent(IEnumerable<ErmState> states)
        {
            States = states;
        }

        public IEnumerable<ErmState> States { get; }
    }
}