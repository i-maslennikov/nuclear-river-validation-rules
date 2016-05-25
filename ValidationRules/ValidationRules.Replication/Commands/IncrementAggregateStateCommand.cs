using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.Commands;
using NuClear.ValidationRules.Storage.Model.Aggregates;

namespace NuClear.ValidationRules.Replication.Commands
{
    public sealed class IncrementAggregateStateCommand : IAggregateCommand
    {
        public IncrementAggregateStateCommand(IReadOnlyCollection<Guid> states)
        {
            States = states;
            AggregateRootType = typeof(ErmState);
        }

        public IReadOnlyCollection<Guid> States { get; }

        public Type AggregateRootType { get; }

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

            return Equals((IncrementAggregateStateCommand)obj);
        }

        public override int GetHashCode()
        {
            return States.Aggregate(0, (accum, guid) => accum ^ guid.GetHashCode());
        }

        private bool Equals(IncrementAggregateStateCommand other)
        {
            return States.Count == other.States.Count && States.Zip(other.States, (x, y) => x.Equals(y)).All(x => x);
        }

    }
}