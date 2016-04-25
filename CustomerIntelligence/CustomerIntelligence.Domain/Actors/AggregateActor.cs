using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;

namespace NuClear.CustomerIntelligence.Replication.Actors
{
    public class AggregateActor : IAggregateActor
    {
        private readonly LeafToRootActor _leafToRootActor;
        private readonly RootToLeafActor _rootToLeafActor;
        private readonly SubrootToLeafActor _subrootToLeafActor;

        public AggregateActor(IAggregateRootActor aggregateRootActor)
        {
            _leafToRootActor = new LeafToRootActor(aggregateRootActor);
            _rootToLeafActor = new RootToLeafActor(aggregateRootActor);
            _subrootToLeafActor = new SubrootToLeafActor(aggregateRootActor.GetEntityActors());
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            var destroyAggregateCommands = commands.OfType<DestroyAggregateCommand>().Distinct().ToArray();
            events.AddRange(_leafToRootActor.ExecuteCommands(destroyAggregateCommands));

            var initializeAggregateCommands = commands.OfType<InitializeAggregateCommand>().Distinct().ToArray();
            events.AddRange(_rootToLeafActor.ExecuteCommands(initializeAggregateCommands));

            var recalculateAggregateCommands = commands.OfType<RecalculateAggregateCommand>().Distinct().ToArray();
            events.AddRange(_rootToLeafActor.ExecuteCommands(recalculateAggregateCommands));

            var recalculateEntityCommands = commands.OfType<RecalculateEntityCommand>().Distinct().ToArray();
            events.AddRange(_subrootToLeafActor.ExecuteCommands(recalculateEntityCommands));

            return events;
        }
    }
}