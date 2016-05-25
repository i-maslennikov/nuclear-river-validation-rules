using System;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class IncrementStateCommand : ICommand
    {
        public IncrementStateCommand(Guid state)
        {
            State = state;
        }

        public Guid State { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((IncrementStateCommand)obj);
        }

        public override int GetHashCode()
        {
            return State.GetHashCode();
        }

        private bool Equals(IncrementStateCommand other)
        {
            return State.Equals(other.State);
        }
    }
}