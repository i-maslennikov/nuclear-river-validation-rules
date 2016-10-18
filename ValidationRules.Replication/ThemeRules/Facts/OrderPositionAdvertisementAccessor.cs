using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Facts
{
    public sealed class OrderPositionAdvertisementAccessor : IStorageBasedDataObjectAccessor<OrderPositionAdvertisement>, IDataChangesHandler<OrderPositionAdvertisement>
    {
        private readonly IQuery _query;

        public OrderPositionAdvertisementAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrderPositionAdvertisement> GetSource()
            => _query.For(Specs.Find.Erm.OrderPositionAdvertisements())
            .Where(x => x.ThemeId != null)
            .Select(x => new OrderPositionAdvertisement
            {
                Id = x.Id,
                OrderPositionId = x.OrderPositionId,
                ThemeId = x.ThemeId.Value,
            });

        public FindSpecification<OrderPositionAdvertisement> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<OrderPositionAdvertisement>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderPositionAdvertisement> dataObjects)
        {
            var orderPositionIds = dataObjects.Select(x => x.OrderPositionId).ToArray();

            var orderIds =
                from op in _query.For<OrderPosition>().Where(x => orderPositionIds.Contains(x.Id))
                select op.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } }.ToArray();
        }
    }
}