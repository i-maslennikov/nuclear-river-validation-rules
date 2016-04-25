using System.Collections.Generic;

namespace NuClear.Replication.Core.Actors
{
    public class LeafToRootActor : IActor
    {
        private readonly IAggregateRootActor _aggregateRootActor;

        public LeafToRootActor(IAggregateRootActor aggregateRootActor)
        {
            _aggregateRootActor = aggregateRootActor;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            foreach (var entityActor in _aggregateRootActor.GetEntityActors())
            {
                foreach (var valueObjectActor in entityActor.GetValueObjectActors())
                {
                    valueObjectActor.ExecuteCommands(commands);
                }

                events.AddRange(entityActor.ExecuteCommands(commands));
            }

            foreach (var valueObjectActor in _aggregateRootActor.GetValueObjectActors())
            {
                valueObjectActor.ExecuteCommands(commands);
            }

            events.AddRange(_aggregateRootActor.ExecuteCommands(commands));

            return events;
        }
    }
}