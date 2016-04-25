using System.Collections.Generic;

using NuClear.River.Common.Metadata;

namespace NuClear.Replication.Core.API.Actors
{
    public sealed class SubrootToLeafActor : IActor
    {
        private readonly IReadOnlyCollection<IEntityActor> _entityActors;

        public SubrootToLeafActor(IReadOnlyCollection<IEntityActor> entityActors)
        {
            _entityActors = entityActors;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            foreach (var entityActor in _entityActors)
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