using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Aggregates
{
    public class InitializeAggregateActor : IActor
    {
        private readonly ICreateDataObjectsActor _createDataObjectsesActor;
        private readonly IReadOnlyCollection<IReplaceDataObjectsActor> _replaceDataObjectsActors;

        protected InitializeAggregateActor(IAggregateActorsFactory aggregateActorsFactory)
        {
            _createDataObjectsesActor = aggregateActorsFactory.CreateRootInitializationActor();
            _replaceDataObjectsActors = aggregateActorsFactory.CreateValueObjectsActors();
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();
            events.AddRange(_createDataObjectsesActor.ExecuteCommands(commands));

            foreach (var actor in _replaceDataObjectsActors)
            {
                events.AddRange(actor.ExecuteCommands(commands));
            }

            return events;
        }
    }
}