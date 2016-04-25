using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core.API.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.API.Actors
{
    public abstract class DataObjectsActorBase<TDataObject> : IActor
        where TDataObject : class
    {
        private readonly IQuery _query;
        private readonly IStorageBasedDataObjectAccessor<TDataObject> _storageBasedDataObjectAccessor;
        private readonly MapToObjectsSpecProvider<TDataObject, TDataObject> _mapSpecificationProviderForSource;
        private readonly MapToObjectsSpecProvider<TDataObject, TDataObject> _mapSpecificationProviderForTarget;

        protected DataObjectsActorBase(IQuery query, IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
        {
            _query = query;
            _storageBasedDataObjectAccessor = storageBasedDataObjectAccessor;

            _mapSpecificationProviderForSource = specification => new MapSpecification<IQuery, IEnumerable<TDataObject>>(q => _storageBasedDataObjectAccessor.GetSource().Where(specification));

            var mapSpecification = new MapSpecification<IQuery, IQueryable<TDataObject>>(q => q.For<TDataObject>());
            _mapSpecificationProviderForTarget = specification => new MapSpecification<IQuery, IEnumerable<TDataObject>>(q => mapSpecification.Map(q).Where(specification));
        }

        public abstract IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands);

        protected MergeResult<TDataObject> DetectChanges(IReadOnlyCollection<ICommand> commands, IEqualityComparer<TDataObject> equalityComparer)
        {
            var dataChangesDetector = new DataChangesDetector<TDataObject>(
                _mapSpecificationProviderForSource,
                _mapSpecificationProviderForTarget,
                equalityComparer,
                _query);

            return dataChangesDetector.DetectChanges(_storageBasedDataObjectAccessor.GetFindSpecification(commands));
        }

        protected MergeResult<TDataObject> DetectChanges(
            IReadOnlyCollection<ICommand> commands,
            IEqualityComparer<TDataObject> identityEqualityComparer,
            IEqualityComparer<TDataObject> completEqualityComparer)
        {
            var dataChangesDetector = new TwoPhaseDataChangesDetector<TDataObject>(
                _mapSpecificationProviderForSource,
                _mapSpecificationProviderForTarget,
                identityEqualityComparer,
                completEqualityComparer,
                _query);

            return dataChangesDetector.DetectChanges(_storageBasedDataObjectAccessor.GetFindSpecification(commands));
        }
    }
}