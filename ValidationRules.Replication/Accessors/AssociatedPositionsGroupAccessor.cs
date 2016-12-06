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
    public sealed class AssociatedPositionsGroupAccessor : IStorageBasedDataObjectAccessor<AssociatedPositionsGroup>, IDataChangesHandler<AssociatedPositionsGroup>
    {
        private readonly IQuery _query;

        public AssociatedPositionsGroupAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<AssociatedPositionsGroup> GetSource() => _query
            .For<Erm::AssociatedPositionsGroup>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new AssociatedPositionsGroup
            {
                Id = x.Id,
                PricePositionId = x.PricePositionId,
            });

        public FindSpecification<AssociatedPositionsGroup> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<AssociatedPositionsGroup>.Create(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<AssociatedPositionsGroup> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<AssociatedPositionsGroup> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<AssociatedPositionsGroup> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<AssociatedPositionsGroup> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var priceIds = from associatedPositionsGroup in _query.For<AssociatedPositionsGroup>().Where(x => ids.Contains(x.Id))
                           from pricePosition in _query.For<PricePosition>().Where(x => x.Id == associatedPositionsGroup.PricePositionId)
                           select pricePosition.PriceId;

            var orderIds = from associatedPositionsGroup in _query.For<AssociatedPositionsGroup>().Where(x => ids.Contains(x.Id))
                           from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == associatedPositionsGroup.PricePositionId)
                           select orderPosition.OrderId;

            return new EventCollectionHelper
                {
                    { typeof(Order), orderIds.Distinct() },
                    { typeof(Price), priceIds.Distinct() },
                };
        }
    }
}