using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Facts
{
    public sealed class SyncFactsActor<TDataObject> : IActor
        where TDataObject : class
    {
        private readonly MapToObjectsSpecProvider<TDataObject, TDataObject> _mapSpecificationProviderForSource;
        private readonly MapToObjectsSpecProvider<TDataObject, TDataObject> _mapSpecificationProviderForTarget;
        private readonly IQuery _query;
        private readonly IStorageBasedFactActor<TDataObject> _storageBasedFactActor;
        private readonly IBulkRepository<TDataObject> _bulkRepository;

        public SyncFactsActor(IQuery query, IBulkRepository<TDataObject> bulkRepository, IStorageBasedFactActor<TDataObject> storageBasedFactActor)
        {
            _query = query;
            _bulkRepository = bulkRepository;
            _storageBasedFactActor = storageBasedFactActor;

            _mapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<TDataObject>>(q => _storageBasedFactActor.GetDataObjectsSource(q).Where(specification));

            var mapSpecification = new MapSpecification<IQuery, IQueryable<TDataObject>>(q => q.For<TDataObject>());
            _mapSpecificationProviderForTarget = specification => new MapSpecification<IQuery, IEnumerable<TDataObject>>(q => mapSpecification.Map(q).Where(specification));
        }


        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var events = new List<IEvent>();

            var changes = DetectChanges(commands);

            var factsToCreate = changes.Difference.ToArray();
            var factsToUpdate = changes.Intersection.ToArray();
            var factsToDelete = changes.Complement.ToArray();

            _bulkRepository.Create(factsToCreate);
            events.AddRange(_storageBasedFactActor.HandleCreates(factsToCreate));

            events.AddRange(_storageBasedFactActor.HandleReferences(_query, factsToUpdate));
            _bulkRepository.Update(factsToCreate);
            events.AddRange(_storageBasedFactActor.HandleUpdates(factsToUpdate));


            _bulkRepository.Delete(factsToCreate);
            events.AddRange(_storageBasedFactActor.HandleDeletes(factsToDelete));

            return events;
        }

        private MergeResult<TDataObject> DetectChanges(IReadOnlyCollection<ICommand> commands)
        {
            var dataChangesDetector = new DataChangesDetector<TDataObject>(
                _mapSpecificationProviderForSource,
                _mapSpecificationProviderForTarget,
                _storageBasedFactActor.DataObjectEqualityComparer,
                _query);

            return dataChangesDetector.DetectChanges(_storageBasedFactActor.GetDataObjectsFindSpecification(commands));
        }
    }
}