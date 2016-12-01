using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class AssociatedPositionAccessor : IStorageBasedDataObjectAccessor<AssociatedPosition>, IDataChangesHandler<AssociatedPosition>
    {
        private readonly IQuery _query;

        public AssociatedPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AssociatedPosition> GetSource() => _query
            .For<Erm::AssociatedPosition>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new AssociatedPosition
                {
                    Id = x.Id,
                    AssociatedPositionsGroupId = x.AssociatedPositionsGroupId,
                    PositionId = x.PositionId,
                    ObjectBindingType = x.ObjectBindingType,
                });

        public FindSpecification<AssociatedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<AssociatedPosition>.Create(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AssociatedPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AssociatedPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AssociatedPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AssociatedPosition> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var orderIds = from associatedPosition in _query.For<AssociatedPosition>().Where(x => ids.Contains(x.Id))
                           from associatedPositionGroup in _query.For<AssociatedPositionsGroup>().Where(x => x.Id == associatedPosition.AssociatedPositionsGroupId)
                           from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == associatedPositionGroup.PricePositionId)
                           select orderPosition.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}