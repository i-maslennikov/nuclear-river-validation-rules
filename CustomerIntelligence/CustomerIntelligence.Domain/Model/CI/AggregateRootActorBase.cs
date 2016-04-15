using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public abstract class AggregateRootActorBase<TDataObject> : IActor
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<TDataObject> _projectCategoryStatisticsBulkRepository;
        private readonly IStorageBasedDataObjectAccessor<TDataObject> _storageBasedDataObjectAccessor;

        protected AggregateRootActorBase(IQuery query, IBulkRepository<TDataObject> projectCategoryStatisticsBulkRepository, IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
        {
            _query = query;
            _projectCategoryStatisticsBulkRepository = projectCategoryStatisticsBulkRepository;
            _storageBasedDataObjectAccessor = storageBasedDataObjectAccessor;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            var commandGroups = commands.GroupBy(x => x.GetType())
                                        .OrderByDescending(x => x.Key, new AggregateCommandPriorityComparer());
            foreach (var commandGroup in commandGroups)
            {
                IActor actor;
                if (commandGroup.Key == typeof(InitializeAggregateCommand))
                {
                    actor = new CreateDataObjectsActor<TDataObject>(_query, _projectCategoryStatisticsBulkRepository, _storageBasedDataObjectAccessor);
                    actor.ExecuteCommands(commandGroup.ToArray());
                }
                else if (commandGroup.Key == typeof(RecalculateAggregateCommand))
                {
                    actor = new SyncDataObjectsActor<TDataObject>(_query, _projectCategoryStatisticsBulkRepository, _storageBasedDataObjectAccessor);
                    actor.ExecuteCommands(commandGroup.ToArray());
                }
                else if (commandGroup.Key == typeof(DestroyAggregateCommand))
                {
                    actor = new DeleteDataObjectsActor<TDataObject>(_query, _projectCategoryStatisticsBulkRepository, _storageBasedDataObjectAccessor);
                    actor.ExecuteCommands(commandGroup.ToArray());
                }
                else
                {
                    throw new InvalidOperationException($"The command of type {commandGroup.Key.Name} is not supported by {typeof(TDataObject).Name} aggregate");
                }
            }

            return events;
        }
    }
}