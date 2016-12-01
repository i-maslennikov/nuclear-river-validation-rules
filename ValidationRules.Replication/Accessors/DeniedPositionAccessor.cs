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
    public sealed class DeniedPositionAccessor : IStorageBasedDataObjectAccessor<DeniedPosition>, IDataChangesHandler<DeniedPosition>
    {
        private readonly IQuery _query;

        public DeniedPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<DeniedPosition> GetSource() => _query
            .For<Erm::DeniedPosition>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new DeniedPosition
                {
                    Id = x.Id,
                    PriceId = x.PriceId,
                    PositionDeniedId = x.PositionDeniedId,
                    PositionId = x.PositionId,
                    ObjectBindingType = x.ObjectBindingType,
                });

        public FindSpecification<DeniedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<DeniedPosition>.Create(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<DeniedPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<DeniedPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<DeniedPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<DeniedPosition> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            // Умное решение - пересчитать все заказы, в order position которых указаны price position, связанные с изменённой denied.
            // И плюс к тому пересчитать все заказы, оформленные по прайс-листу изменённой denied position, в opa которого указны позиции изменных denied position.
            // Но я выбрал решение проще - прересчитать все заказы по прайс-листам изменённых denied position, ибо нефиг менять опубликованный прайс-лист.

            var orderIds = from deniedPosition in _query.For<DeniedPosition>().Where(x => ids.Contains(x.Id))
                           from pricePosition in _query.For<PricePosition>().Where(x => x.PriceId == deniedPosition.PriceId)
                           from orderPosition in _query.For<OrderPosition>().Where(x => x.PricePositionId == pricePosition.Id)
                           select orderPosition.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}