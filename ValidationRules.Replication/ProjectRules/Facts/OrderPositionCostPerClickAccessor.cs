using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Facts
{
    public sealed class OrderPositionCostPerClickAccessor : IStorageBasedDataObjectAccessor<OrderPositionCostPerClick>, IDataChangesHandler<OrderPositionCostPerClick>
    {
        private readonly IQuery _query;

        public OrderPositionCostPerClickAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrderPositionCostPerClick> GetSource()
            => from cpc in _query.For<Storage.Model.Erm.OrderPositionCostPerClick>()
               where cpc.BidIndex == _query.For<Storage.Model.Erm.OrderPositionCostPerClick>().Where(x => x.CategoryId == cpc.CategoryId && x.OrderPositionId == cpc.OrderPositionId).Max(x => x.BidIndex)
               select new OrderPositionCostPerClick
                   {
                       OrderPositionId = cpc.OrderPositionId,
                       CategoryId = cpc.CategoryId,
                       Amount = cpc.Amount,
                   };

        public FindSpecification<OrderPositionCostPerClick> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<OrderPositionCostPerClick>(x => ids.Contains(x.OrderPositionId));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderPositionCostPerClick> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderPositionCostPerClick> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderPositionCostPerClick> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderPositionCostPerClick> dataObjects)
        {
            var orderPositionIds = dataObjects.Select(x => x.OrderPositionId);

            var orderIds =
                from op in _query.For<OrderPosition>().Where(x => orderPositionIds.Contains(x.Id))
                select op.OrderId;

            return new EventCollectionHelper { { typeof(Order), orderIds.Distinct() } };
        }
    }
}