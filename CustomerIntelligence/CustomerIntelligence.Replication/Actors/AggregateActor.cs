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

            IReadOnlyCollection<ICommand> commandsToExecute =
                commands.OfType<DestroyAggregateCommand>()
                        .Distinct()
                        .Aggregate(new List<ICommand>(),
                                   (result, next) =>
                                       {
                                           result.Add(new DeleteDataObjectCommand(next.AggregateRootType, next.AggregateRootId));
                                           result.Add(new ReplaceValueObjectCommand(next.AggregateRootId));
                                           return result;
                                       })
                        .ToArray();
            events.AddRange(_leafToRootActor.ExecuteCommands(commandsToExecute));

            commandsToExecute = commands.OfType<InitializeAggregateCommand>()
                                        .Distinct()
                                        .Aggregate(new List<ICommand>(),
                                                   (result, next) =>
                                                       {
                                                           result.Add(new CreateDataObjectCommand(next.AggregateRootType, next.AggregateRootId));
                                                           result.Add(new ReplaceValueObjectCommand(next.AggregateRootId));
                                                           return result;
                                                       })
                                        .ToArray();
            events.AddRange(_rootToLeafActor.ExecuteCommands(commandsToExecute));

            commandsToExecute = commands.OfType<RecalculateAggregateCommand>()
                                        .Distinct()
                                        .Aggregate(new List<ICommand>(),
                                                   (result, next) =>
                                                       {
                                                           result.Add(new SyncDataObjectCommand(next.AggregateRootType, next.AggregateRootId));
                                                           result.Add(new ReplaceValueObjectCommand(next.AggregateRootId));
                                                           return result;
                                                       })
                                        .ToArray();
            events.AddRange(_rootToLeafActor.ExecuteCommands(commandsToExecute));

            commandsToExecute = commands.OfType<RecalculateEntityCommand>()
                                        .Distinct()
                                        .Aggregate(new List<ICommand>(),
                                                   (result, next) =>
                                                       {
                                                           result.Add(new SyncDataObjectCommand(next.EntityType, next.EntityId));
                                                           result.Add(new ReplaceValueObjectCommand(next.AggregateRootId, next.EntityId));
                                                           return result;
                                                       })
                                        .ToArray();
            events.AddRange(_subrootToLeafActor.ExecuteCommands(commandsToExecute));

            return events;
        }
    }
}