using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Facts
{
    public sealed class OrderPositionAccessor : IStorageBasedDataObjectAccessor<OrderPosition>, IDataChangesHandler<OrderPosition>
    {
        private readonly IQuery _query;

        public OrderPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrderPosition> GetSource()
            => from orderPosition in _query.For(Specs.Find.Erm.OrderPositions())
               select new OrderPosition
                   {
                       Id = orderPosition.Id,
                       OrderId = orderPosition.OrderId,
                       PricePositionId = orderPosition.PricePositionId
               };

        public FindSpecification<OrderPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<OrderPosition>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderPosition> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderPosition> dataObjects)
        {
            // OrderId ������� �� ��������
            var orderIds = dataObjects.Select(x => x.OrderId);
            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}