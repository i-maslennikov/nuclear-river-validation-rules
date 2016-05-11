using System.Collections.Generic;

using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.Replication.Core.Actors
{
    public abstract class DataObjectsActorBase<TDataObject> : IActor
        where TDataObject : class
    {
        private readonly IStorageBasedDataObjectAccessor<TDataObject> _storageBasedDataObjectAccessor;
        private readonly MapToObjectsSpecProvider<TDataObject, TDataObject> _mapSpecificationProviderForSource;
        private readonly MapToObjectsSpecProvider<TDataObject, TDataObject> _mapSpecificationProviderForTarget;

        protected DataObjectsActorBase(IQuery query, IStorageBasedDataObjectAccessor<TDataObject> storageBasedDataObjectAccessor)
        {
            _storageBasedDataObjectAccessor = storageBasedDataObjectAccessor;

            _mapSpecificationProviderForSource = specification => _storageBasedDataObjectAccessor.GetSource().Where(specification);
            _mapSpecificationProviderForTarget = specification => query.For<TDataObject>().Where(specification);
        }

        public abstract IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands);

        protected MergeResult<TDataObject> DetectChanges(IReadOnlyCollection<ICommand> commands, IEqualityComparer<TDataObject> equalityComparer)
        {
            var dataChangesDetector = new DataChangesDetector<TDataObject>(
                _mapSpecificationProviderForSource,
                _mapSpecificationProviderForTarget,
                equalityComparer);

            return dataChangesDetector.DetectChanges(_storageBasedDataObjectAccessor.GetFindSpecification(commands));
        }

        protected MergeResult<TDataObject> DetectChanges(
            IReadOnlyCollection<ICommand> commands,
            IEqualityComparer<TDataObject> identityEqualityComparer,
            IEqualityComparer<TDataObject> completeEqualityComparer)
        {
            var dataChangesDetector = new TwoPhaseDataChangesDetector<TDataObject>(
                _mapSpecificationProviderForSource,
                _mapSpecificationProviderForTarget,
                identityEqualityComparer,
                completeEqualityComparer);

            return dataChangesDetector.DetectChanges(_storageBasedDataObjectAccessor.GetFindSpecification(commands));
        }
    }
}