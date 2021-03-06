﻿using System.Collections.Generic;
using System.Linq;

using LinqToDB.Common;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.Commands;
using NuClear.Telemetry.Probing;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.Replication
{
    public class AggregateActor : IAggregateActor
    {
        private readonly RootToLeafActor _rootToLeafActor;
        private readonly IAggregateRootActor _aggregateRootActor;

        public AggregateActor(IAggregateRootActor aggregateRootActor)
        {
            _aggregateRootActor = aggregateRootActor;
            _rootToLeafActor = new RootToLeafActor(aggregateRootActor);
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var aggregateCommands =
                commands.OfType<IAggregateCommand>()
                        .Where(x => x.AggregateRootType == _aggregateRootActor.EntityType)
                        .Distinct()
                        .ToArray();

            IEnumerable<IEvent> events = Array<IEvent>.Empty;

            if (!aggregateCommands.Any())
            {
                return Array<IEvent>.Empty;
            }

            var aggregateNameParts = _aggregateRootActor.EntityType.FullName.Split('.').Reverse().ToArray();
            using (Probe.Create("Aggregate", aggregateNameParts[2], aggregateNameParts[0]))
            {
                var recalculateCommands =
                    aggregateCommands.OfType<AggregateCommand.Recalculate>()
                                     .SelectMany(next => new ICommand[]
                                                     {
                                                         new SyncDataObjectCommand(next.AggregateRootType, next.AggregateRootId),
                                                         new ReplaceValueObjectCommand(next.AggregateRootId)
                                                     })
                                     .ToArray();
                events = events.Union(_rootToLeafActor.ExecuteCommands(recalculateCommands));

                var recalculatePeriodCommands =
                    aggregateCommands.OfType<RecalculatePeriodCommand>()
                                     .SelectMany(next => new ICommand[]
                                         {
                                             new SyncPeriodCommand(next.AggregateRootType, next.Point.Date)
                                         })
                                     .ToArray();
                events = events.Union(_rootToLeafActor.ExecuteCommands(recalculatePeriodCommands));

                return events.ToArray();
            }
        }
    }
}