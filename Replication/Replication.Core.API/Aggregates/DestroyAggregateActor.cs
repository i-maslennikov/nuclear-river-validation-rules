using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Aggregates
{
    public class DestroyAggregateActor : IActor
    {
        private readonly IDeleteDataObjectsActor _deleteDataObjectsActor;
        private readonly IReadOnlyCollection<IReplaceDataObjectsActor> _replaceDataObjectsActors;

        public DestroyAggregateActor(IAggregateActorsFactory aggregateActorsFactory)
        {
            _deleteDataObjectsActor = aggregateActorsFactory.CreateRootDestructionActor();
            _replaceDataObjectsActors = aggregateActorsFactory.CreateValueObjectsActors();
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            foreach (var actor in _replaceDataObjectsActors)
            {
                events.AddRange(actor.ExecuteCommands(commands));
            }

            events.AddRange(_deleteDataObjectsActor.ExecuteCommands(commands));

            return events;
        }
    }
}