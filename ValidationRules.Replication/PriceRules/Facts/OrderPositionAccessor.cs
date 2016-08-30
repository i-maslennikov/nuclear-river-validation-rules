using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Facts
{
    public sealed class OrderPositionAccessor : IStorageBasedDataObjectAccessor<OrderPosition>, IDataChangesHandler<OrderPosition>
    {
        private readonly IQuery _query;

        public OrderPositionAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrderPosition> GetSource() => Specs.Map.Erm.ToFacts.OrderPosition.Map(_query);

        public FindSpecification<OrderPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<OrderPosition>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrderPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrderPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrderPosition> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrderPosition> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<OrderPosition>(x => ids.Contains(x.Id));

            var orderIds = (from orderPosition in _query.For(specification)
                            select orderPosition.OrderId)
                            .Distinct()
                            .ToArray();

            return orderIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Order), x))
                          .ToArray();
        }
    }
}