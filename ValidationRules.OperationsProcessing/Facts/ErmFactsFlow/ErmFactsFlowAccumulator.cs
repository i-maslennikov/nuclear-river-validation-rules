using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing;
using NuClear.Storage.API.Readings;
using NuClear.Tracing.API;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.ErmFactsFlow
{
    public sealed class ErmFactsFlowAccumulator : MessageProcessingContextAccumulatorBase<ErmFactsFlow, TrackedUseCase, AggregatableMessage<ICommand>>
    {
        private const int TotalWaitMilliseconds = 60000;

        private readonly IQuery _query;
        private readonly ITracer _tracer;
        private readonly ICommandFactory<TrackedUseCase> _commandFactory;

        public ErmFactsFlowAccumulator(IQuery query, ITracer tracer)
        {
            _query = query;
            _tracer = tracer;
            _commandFactory = new FactsCommandFactory();
        }

        protected override AggregatableMessage<ICommand> Process(TrackedUseCase trackedUseCase)
        {
            var commands = _commandFactory.CreateCommands(trackedUseCase);
            return new AggregatableMessage<ICommand>
            {
                TargetFlow = MessageFlow,
                Commands = commands,
            };
        }

        private sealed class FactsCommandFactory : ICommandFactory<TrackedUseCase>
        {
            public IReadOnlyCollection<ICommand> CreateCommands(TrackedUseCase trackedUseCase)
            {
                var changes = trackedUseCase.Operations.SelectMany(x => x.AffectedEntities.Changes);
                return changes.SelectMany(x => CommandsForEntityType(x.Key.Id, x.Value.Keys))
                              .Concat(new[]
                                  {
                                      new IncrementErmStateCommand(new[]
                                          {
                                              new ErmState(trackedUseCase.Id,
                                                           trackedUseCase.Context.Finished.UtcDateTime)
                                          })
                                  })
                              .ToList();
            }

            private static IEnumerable<ICommand> CommandsForEntityType(int entityTypeId, IEnumerable<long> ids)
            {
                var commands = Enumerable.Empty<ICommand>();

                if (EntityTypeMap.TryGetErmFactTypes(entityTypeId, out var factTypes))
                {
                    var syncDataObjectCommands = from factType in factTypes
                                                 from id in ids
                                                 select new SyncDataObjectCommand(factType, id);

                    commands = commands.Concat(syncDataObjectCommands);
                }

                return commands;
            }
        }
    }
}