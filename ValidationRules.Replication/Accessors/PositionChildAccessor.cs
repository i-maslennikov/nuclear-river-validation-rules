using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class PositionChildAccessor : IStorageBasedDataObjectAccessor<PositionChild>, IDataChangesHandler<PositionChild>
    {
        private readonly IQuery _query;

        public PositionChildAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<PositionChild> GetSource() => _query
            .For<Erm::PositionChild>()
            .Select(x => new PositionChild
                {
                    MasterPositionId = x.MasterPositionId,
                    ChildPositionId = x.ChildPositionId,
                });

        public FindSpecification<PositionChild> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<PositionChild>.Contains(x => x.MasterPositionId, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<PositionChild> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<PositionChild> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<PositionChild> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<PositionChild> dataObjects)
        {
            var positionIds = dataObjects.Select(x => x.MasterPositionId).ToList();

            var orderIdsFromPricePosition =
                  from pricePosition in _query.For<PricePosition>().Where(x => positionIds.Contains(x.PositionId))
                  from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == pricePosition.Id)
                  select orderPosition.OrderId;

            return new EventCollectionHelper<PositionChild> { { typeof(Order), orderIdsFromPricePosition } };
        }
    }
}