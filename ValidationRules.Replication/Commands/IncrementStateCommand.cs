using System;
using System.Collections.Generic;

using NuClear.Replication.Core;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class IncrementStateCommand : ICommand
    {
        public IncrementStateCommand(IReadOnlyCollection<Guid> states)
        {
            States = states;
        }

        public IReadOnlyCollection<Guid> States { get; }

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
            return States.GetHashCode();
        }

        private bool Equals(IncrementStateCommand other)
        {
            return ReferenceEquals(States, other.States);
        }
    }
}