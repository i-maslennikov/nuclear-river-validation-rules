using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.Commands;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.Replication
{
    public class AggregateActor : IAggregateActor
    {
        private readonly LeafToRootActor _leafToRootActor;
        private readonly RootToLeafActor _rootToLeafActor;
        private readonly SubrootToLeafActor _subrootToLeafActor;
        private readonly IAggregateRootActor _aggregateRootActor;

        public AggregateActor(IAggregateRootActor aggregateRootActor)
        {
            _aggregateRootActor = aggregateRootActor;
            _leafToRootActor = new LeafToRootActor(aggregateRootActor);
            _rootToLeafActor = new RootToLeafActor(aggregateRootActor);
            _subrootToLeafActor = new SubrootToLeafActor(aggregateRootActor.GetEntityActors());
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var aggregateCommands =
                commands.OfType<IAggregateCommand>()
                        .Where(x => x.AggregateRootType == _aggregateRootActor.EntityType)
                        .Distinct()
                        .ToArray();

            var events = new List<IEvent>();

            if (!aggregateCommands.Any())
            {
                return events;
            }

            using (Probe.Create("Aggregate", _aggregateRootActor.EntityType.Name))
            {
                IReadOnlyCollection<ICommand> commandsToExecute =
                    aggregateCommands.OfType<DestroyAggregateCommand>()
                                     .Aggregate(new List<ICommand>(),
                                         (result, next) =>
                                             {
                                                 result.Add(new DeleteDataObjectCommand(next.AggregateRootType, next.AggregateRootId));
                                                 result.Add(new ReplaceValueObjectCommand(next.AggregateRootId));
                                                 return result;
                                             })
                                     .ToArray();
                events.AddRange(_leafToRootActor.ExecuteCommands(commandsToExecute));

                commandsToExecute =
                    aggregateCommands.OfType<InitializeAggregateCommand>()
                                     .Aggregate(new List<ICommand>(),
                                         (result, next) =>
                                             {
                                                 result.Add(new CreateDataObjectCommand(next.AggregateRootType, next.AggregateRootId));
                                                 result.Add(new ReplaceValueObjectCommand(next.AggregateRootId));
                                                 return result;
                                             })
                                     .ToArray();
                events.AddRange(_rootToLeafActor.ExecuteCommands(commandsToExecute));

                commandsToExecute =
                    aggregateCommands.OfType<RecalculateAggregateCommand>()
                                     .Aggregate(new List<ICommand>(),
                                         (result, next) =>
                                             {
                                                 result.Add(new SyncDataObjectCommand(next.AggregateRootType, next.AggregateRootId));
                                                 result.Add(new ReplaceValueObjectCommand(next.AggregateRootId));
                                                 return result;
                                             })
                                     .ToArray();
                events.AddRange(_rootToLeafActor.ExecuteCommands(commandsToExecute));

                commandsToExecute =
                    aggregateCommands.OfType<RecalculateEntityCommand>()
                                     .Aggregate(new List<ICommand>(),
                                         (result, next) =>
                                             {
                                                 result.Add(new SyncDataObjectCommand(next.EntityType, next.EntityId));
                                                 result.Add(new ReplaceValueObjectCommand(next.AggregateRootId, next.EntityId));
                                                 return result;
                                             })
                                     .ToArray();
                events.AddRange(_subrootToLeafActor.ExecuteCommands(commandsToExecute));

                commandsToExecute =
                    aggregateCommands.OfType<RecalculatePeriodAggregateCommand>()
                                     .Aggregate(new List<ICommand>(),
                                         (result, next) =>
                                             {
                                                 result.Add(new SyncPeriodDataObjectCommand(next.PeriodKey));
                                                 result.Add(new ReplacePeriodValueObjectCommand(next.PeriodKey));
                                                 return result;
                                             })
                                     .ToArray();
                events.AddRange(_subrootToLeafActor.ExecuteCommands(commandsToExecute));

                return events;
            }
        }
    }
}