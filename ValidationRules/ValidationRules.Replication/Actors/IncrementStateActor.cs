using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;

namespace NuClear.ValidationRules.Replication.Actors
{
    public sealed class IncrementStateActor : IActor
    {
        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var states = commands.OfType<IncrementStateCommand>().Select(command => command.State).ToArray();
            return new[] { new StateIncrementedEvent(states) };
        }
    }
}