using System.Collections.Generic;
using System.Linq;


using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class PositionAccessor : IStorageBasedDataObjectAccessor<Position>, IDataChangesHandler<Position>
    {
        private readonly IQuery _query;

        public PositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Position> GetSource() => Specs.Map.Erm.ToFacts.Position.Map(_query);

        public FindSpecification<Position> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Position>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Position> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Position), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Position> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Position), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Position> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Position), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Position> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Order>(x => ids.Contains(x.Id));

            var viaOrderPositionAdvertisement
                = from position in _query.For(specification)
                  join opa in _query.For<OrderPositionAdvertisement>() on position.Id equals opa.PositionId
                  join orderPosition in _query.For<OrderPosition>() on opa.OrderPositionId equals orderPosition.Id
                  select orderPosition.OrderId;

            var viaPricePosition
                = from position in _query.For(specification)
                  join pp in _query.For<PricePosition>() on position.Id equals pp.PositionId
                  join orderPosition in _query.For<OrderPosition>() on pp.Id equals orderPosition.PricePositionId
                  select orderPosition.OrderId;

            var orderIds = viaOrderPositionAdvertisement
                           .Concat(viaPricePosition)
                           .Distinct()
                           .ToArray();


            return orderIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Order), x)).ToArray();
        }
    }
}