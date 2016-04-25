using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Actors;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Replication.Core.API.Equality;
using NuClear.Storage.API.Readings;

namespace NuClear.CustomerIntelligence.Domain.Actors
{
    public abstract class EntityActorBase<TDataObject> : IEntityActor
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<TDataObject> _projectCategoryStatisticsBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IStorageBasedDataObjectAccessor<TDataObject> _storageBasedDataObjectAccessor;

        protected EntityActorBase(
            IQuery query,
            IBulkRepository<TDataObject> projectCategoryStatisticsBulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
        {
            _query = query;
            _projectCategoryStatisticsBulkRepository = projectCategoryStatisticsBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
            _storageBasedDataObjectAccessor = storageBasedDataObjectAccessor;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var commandTypes = commands.GroupBy(x => x.GetType()).Select(x => x.Key).ToArray();
            if (commandTypes.Length != 1)
            {
                throw new ArgumentException("Single instance of entity actor can execute single commands type only", nameof(commands));
            }

            var commandType = commandTypes[0];
            var events = new List<IEvent>();

            IReadOnlyCollection<ICommand> dataObjectCommands;
            IActor dataObjectActor;
            if (commandType == typeof(InitializeAggregateCommand))
            {
                dataObjectCommands = commands.OfType<InitializeAggregateCommand>()
                                             .Select(x => new CreateDataObjectCommand(x.AggregateRootType, x.AggregateRootId))
                                             .ToArray();

                dataObjectActor = new CreateDataObjectsActor<TDataObject>(
                    _query,
                    _projectCategoryStatisticsBulkRepository,
                    _equalityComparerFactory,
                    _storageBasedDataObjectAccessor);
            }
            else if (commandType == typeof(RecalculateAggregateCommand))
            {
                dataObjectCommands = commands.OfType<RecalculateAggregateCommand>()
                                             .Select(x => new SyncDataObjectCommand(x.AggregateRootType, x.AggregateRootId))
                                             .ToArray();

                dataObjectActor = new SyncDataObjectsActor<TDataObject>(
                    _query,
                    _projectCategoryStatisticsBulkRepository,
                    _equalityComparerFactory,
                    _storageBasedDataObjectAccessor);
            }
            else if (commandType == typeof(RecalculateEntityCommand))
            {
                dataObjectCommands = commands.OfType<RecalculateEntityCommand>()
                                             .Select(x => new SyncDataObjectCommand(x.EntityType, x.EntityId))
                                             .ToArray();

                dataObjectActor = new SyncDataObjectsActor<TDataObject>(
                    _query,
                    _projectCategoryStatisticsBulkRepository,
                    _equalityComparerFactory,
                    _storageBasedDataObjectAccessor);
            }
            else if (commandType == typeof(DestroyAggregateCommand))
            {
                dataObjectCommands = commands.OfType<DestroyAggregateCommand>()
                                             .Select(x => new DeleteDataObjectCommand(x.AggregateRootType, x.AggregateRootId))
                                             .ToArray();

                dataObjectActor = new DeleteDataObjectsActor<TDataObject>(
                    _query,
                    _projectCategoryStatisticsBulkRepository,
                    _equalityComparerFactory,
                    _storageBasedDataObjectAccessor);
            }
            else
            {
                throw new InvalidOperationException($"The command of type {commandType.Name} is not supported by {typeof(TDataObject).Name} aggregate");
            }

            events.AddRange(dataObjectActor.ExecuteCommands(dataObjectCommands));
            return events;
        }

        public abstract IReadOnlyCollection<IActor> GetValueObjectActors();
    }
}