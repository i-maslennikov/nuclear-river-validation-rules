using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Events
{
    public sealed class AmsStateIncrementedEvent : IEvent
    {
        public AmsStateIncrementedEvent(AmsState state)
        {
            State = state;
        }

        public AmsState State { get; }
    }
}