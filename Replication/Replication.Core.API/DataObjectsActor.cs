using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.Facts;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API
{
    public abstract class DataObjectsActor<TDataObject>
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IStorageBasedFactActor<TDataObject> _storageBasedFactActor;
        private readonly MapToObjectsSpecProvider<TDataObject, TDataObject> _mapSpecificationProviderForSource;
        private readonly MapToObjectsSpecProvider<TDataObject, TDataObject> _mapSpecificationProviderForTarget;

        protected DataObjectsActor(IQuery query, IStorageBasedFactActor<TDataObject> storageBasedFactActor)
        {
            _query = query;
            _storageBasedFactActor = storageBasedFactActor;

            _mapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<TDataObject>>(q => _storageBasedFactActor.GetDataObjectsSource(q).Where(specification));

            var mapSpecification = new MapSpecification<IQuery, IQueryable<TDataObject>>(q => q.For<TDataObject>());
            _mapSpecificationProviderForTarget = specification => new MapSpecification<IQuery, IEnumerable<TDataObject>>(q => mapSpecification.Map(q).Where(specification));
        }

        protected MergeResult<TDataObject> DetectChanges(IReadOnlyCollection<ICommand> commands)
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