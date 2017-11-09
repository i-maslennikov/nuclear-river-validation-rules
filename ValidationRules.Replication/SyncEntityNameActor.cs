using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.Commands;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication
{
    public sealed class SyncEntityNameActor<TEntity> : IActor
    {
        private readonly IStorageBasedEntityNameAccessor<TEntity> _accessor;
        private readonly IQuery _query;
        private readonly IBulkRepository<EntityName> _bulkRepository;
        private readonly IEqualityComparer<EntityName> _identityComparer;
        private readonly IEqualityComparer<EntityName> _completeComparer;

        public SyncEntityNameActor(
            IQuery query,
            IBulkRepository<EntityName> bulkRepository,
            IStorageBasedEntityNameAccessor<TEntity> accessor,
            IEqualityComparerFactory equalityComparerFactory) // todo: IDataChangesHandler<EntityName>
        {
            _query = query;
            _accessor = accessor;
            _bulkRepository = bulkRepository;
            _identityComparer = equalityComparerFactory.CreateIdentityComparer<EntityName>();
            _completeComparer = equalityComparerFactory.CreateCompleteComparer<EntityName>();
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var syncDataObjectCommands = commands
                .OfType<ISyncDataObjectCommand>()
                .Where(x => x.DataObjectType == typeof(TEntity))
                .ToList();

            if (syncDataObjectCommands.Count == 0)
            {
                return Array.Empty<IEvent>();
            }

            var dataChangesDetector = new TwoPhaseDataChangesDetector<EntityName>(
                spec => _accessor.GetSource().WhereMatched(spec),
                spec => _query.For<EntityName>().WhereMatched(spec),
                _identityComparer,
                _completeComparer);

            var specification = _accessor.GetFindSpecification(syncDataObjectCommands);
            var changes = dataChangesDetector.DetectChanges(specification);
            _bulkRepository.Delete(changes.Complement);
            _bulkRepository.Create(changes.Difference);
            _bulkRepository.Update(changes.Intersection);

            return Array.Empty<IEvent>();
        }
    }
}