using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Aggregates
{
    public class RecalculateAggregateActor : IActor
    {
        private readonly ISyncDataObjectsActor _syncDataObjectsActor;
        private readonly IReadOnlyCollection<IReplaceDataObjectsActor> _replaceDataObjectsActors;

        public RecalculateAggregateActor(IAggregateActorsFactory aggregateActorsFactory)
        {
            _syncDataObjectsActor = aggregateActorsFactory.CreateRootSyncActor();
            _replaceDataObjectsActors = aggregateActorsFactory.CreateValueObjectsActors();
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            foreach (var actor in _replaceDataObjectsActors)
            {
                events.AddRange(actor.ExecuteCommands(commands));
            }

            events.AddRange(_syncDataObjectsActor.ExecuteCommands(commands));

            return events;
        }
    }
}