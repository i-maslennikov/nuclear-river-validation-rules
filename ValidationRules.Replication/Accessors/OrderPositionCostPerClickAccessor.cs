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
    public sealed class OrderPositionCostPerClickAccessor : IStorageBasedDataObjectAccessor<OrderPositionCostPerClick>, IDataChangesHandler<OrderPositionCostPerClick>
    {
        private readonly IQuery _query;

        public OrderPositionCostPerClickAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrderPositionCostPerClick> GetSource() => _query
            .For<Erm::OrderPositionCostPerClick>()
            .Where(cpc =>
                   cpc.BidIndex == _query.For<Erm::OrderPositionCostPerClick>()
                                         .Where(x => x.CategoryId == cpc.CategoryId && x.OrderPositionId == cpc.OrderPositionId)
                                         .Max(x => x.BidIndex))
            .Select(cpc => new OrderPositionCostPerClick
            {
                OrderPositionId = cpc.OrderPositionId,
                CategoryId = cpc.CategoryId,
                Amount = cpc.Amount,
            });

        public FindSpecification<OrderPositionCostPerClick> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return Specification<OrderPositionCostPerClick>.Create(x => x.OrderPositionId, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderPositionCostPerClick> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderPositionCostPerClick> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderPositionCostPerClick> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderPositionCostPerClick> dataObjects)
            => Array.Empty<IEvent>();
    }
}