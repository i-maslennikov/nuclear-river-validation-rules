using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API
{
    public sealed class ReplaceDataObjectsActor<TDataObject> : IReplaceDataObjectsActor
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<TDataObject> _bulkRepository;
        private readonly IStorageBasedFactActor<TDataObject> _storageBasedFactActor;
        private readonly IMemoryBasedFactActor<TDataObject> _memoryBasedFactActor;

        public ReplaceDataObjectsActor(IQuery query, IBulkRepository<TDataObject> bulkRepository, IMemoryBasedFactActor<TDataObject> memoryBasedFactActor)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _memoryBasedFactActor = memoryBasedFactActor;
        }

        public ReplaceDataObjectsActor(IQuery query, IBulkRepository<TDataObject> bulkRepository, IStorageBasedFactActor<TDataObject> storageBasedFactActor)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _storageBasedFactActor = storageBasedFactActor;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            if (_memoryBasedFactActor != null)
            {
                return ExecuteCommandsForMemoryBased(commands);
            }
            if (_storageBasedFactActor != null)
            {
                return ExecuteCommandsForStorageBased(commands);
            }

            throw new InvalidOperationException();
        }

        private IReadOnlyCollection<IEvent> ExecuteCommandsForMemoryBased(IEnumerable<ICommand> commands)
        {
            var events = new List<IEvent>();
            foreach (var command in commands)
            {
                using (var transaction = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
                    var findSpecification = _memoryBasedFactActor.GetDataObjectsFindSpecification(command);
                    _bulkRepository.Delete(_query.For(findSpecification));

                    var dataObjects = _memoryBasedFactActor.GetDataObjects(command);
                    _bulkRepository.Create(dataObjects);

                    events.AddRange(_memoryBasedFactActor.HandleChanges(dataObjects));
                    transaction.Complete();
                }
            }

            return events;
        }

        private IReadOnlyCollection<IEvent> ExecuteCommandsForStorageBased(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();
            var changes = DetectChanges(commands);

            var toCreate = changes.Difference.ToArray();
            var toDelete = changes.Complement.ToArray();

            _bulkRepository.Delete(toCreate);
            events.AddRange(_storageBasedFactActor.HandleDeletes(toDelete));

            _bulkRepository.Create(toCreate);
            events.AddRange(_storageBasedFactActor.HandleCreates(toCreate));

            return events;
        }

        private MergeResult<TDataObject> DetectChanges(IReadOnlyCollection<ICommand> commands)
        {
            MapToObjectsSpecProvider<TDataObject, TDataObject> mapSpecificationProviderForSource =
                specification => new MapSpecification<IQuery, IEnumerable<TDataObject>>(q => _storageBasedFactActor.GetDataObjectsSource(q).Where(specification));

            var mapSpecification = new MapSpecification<IQuery, IQueryable<TDataObject>>(q => q.For<TDataObject>());
            MapToObjectsSpecProvider<TDataObject, TDataObject> mapSpecificationProviderForTarget =
                specification => new MapSpecification<IQuery, IEnumerable<TDataObject>>(q => mapSpecification.Map(q).Where(specification));

            var dataChangesDetector = new DataChangesDetector<TDataObject>(
                mapSpecificationProviderForSource,
                mapSpecificationProviderForTarget,
                _storageBasedFactActor.DataObjectEqualityComparer,
                _query);

            return dataChangesDetector.DetectChanges(_storageBasedFactActor.GetDataObjectsFindSpecification(commands));
        }
    }
}