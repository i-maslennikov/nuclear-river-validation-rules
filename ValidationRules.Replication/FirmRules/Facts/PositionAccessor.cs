using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Facts
{
    public sealed class PositionAccessor : IStorageBasedDataObjectAccessor<Position>, IDataChangesHandler<Position>
    {
        private readonly IQuery _query;

        public PositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Position> GetSource()
            => from position in _query.For(Specs.Find.Erm.Positions())
               select new Position
               {
                   Id = position.Id,
                   CategoryCode = position.CategoryCode,
                   Platform = position.Platform,
               };

        public FindSpecification<Position> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Position>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Position> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Position> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Position> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Position> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from pricePosition in _query.For<OrderPositionAdvertisement>().Where(x => ids.Contains(x.PositionId))
                from orderPosition in _query.For<OrderPosition>().Where(x => x.Id == pricePosition.OrderPositionId)
                select orderPosition.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}