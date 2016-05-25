using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.Replication.Actors
{
    public sealed class ErmStateAggregateRootActor : IAggregateRootActor
    {
        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var states = commands.OfType<IncrementAggregateStateCommand>().SelectMany(command => command.States).ToArray();
            return states.Any() ? new[] { new StateIncrementedEvent(states) } : Array.Empty<IEvent>();
        }

        public IReadOnlyCollection<IActor> GetValueObjectActors()
            => Array.Empty<IActor>();

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();
    }
}
 