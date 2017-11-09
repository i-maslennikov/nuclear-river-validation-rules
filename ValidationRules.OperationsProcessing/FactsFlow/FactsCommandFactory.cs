using System.Collections.Generic;
using System.Linq;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.Core;
using NuClear.ValidationRules.Replication;
using NuClear.ValidationRules.Replication.Commands;

namespace NuClear.ValidationRules.OperationsProcessing.FactsFlow
{
    public sealed class FactsCommandFactory : ICommandFactory<TrackedUseCase>
    {
        public IReadOnlyCollection<ICommand> CreateCommands(TrackedUseCase message)
        {
            var changes = message.Operations.SelectMany(x => x.AffectedEntities.Changes);
            return changes.SelectMany(x => CommandsForEntityType(x.Key.Id, x.Value.Keys)).Concat(CreateIncrementErmStateCommand(message)).ToList();
        }

        private static IEnumerable<ICommand> CreateIncrementErmStateCommand(TrackedUseCase message)
        {
            return new[] { new IncrementErmStateCommand(new[] { new ErmState(message.Id, message.Context.Finished.UtcDateTime) }) };
        }

        private static IEnumerable<ICommand> CommandsForEntityType(int entityTypeId, IEnumerable<long> ids)
        {
            var commands = Enumerable.Empty<ICommand>();

            if (EntityTypeMap.TryGetFactTypes(entityTypeId, out var factTypes))
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