using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Actors
{
    public sealed class RootToLeafActor : IActor
    {
        private readonly IAggregateRootActor _aggregateRootActor;

        public RootToLeafActor(IAggregateRootActor aggregateRootActor)
        {
            _aggregateRootActor = aggregateRootActor;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();
            events.AddRange(_aggregateRootActor.ExecuteCommands(commands));

            foreach (var valueObjectActor in _aggregateRootActor.GetValueObjectActors())
            {
                events.AddRange(valueObjectActor.ExecuteCommands(commands));
            }

            foreach (var entityActor in _aggregateRootActor.GetEntityActors())
            {
                events.AddRange(entityActor.ExecuteCommands(commands));

                foreach (var valueObjectActor in entityActor.GetValueObjectActors())
                {
                    events.AddRange(valueObjectActor.ExecuteCommands(commands));
                }
            }

            return events;
        }
    }
}