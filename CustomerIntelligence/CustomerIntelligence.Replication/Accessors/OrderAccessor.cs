using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>, IDataChangesHandler<Order>
    {
        private readonly IQuery _query;

        public OrderAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Order> GetSource() => Specs.Map.Erm.ToFacts.Orders.Map(_query);

        public FindSpecification<Order> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Order>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Order> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Order> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Order> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Order> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Order>(x => ids.Contains(x.Id));

            var firmIds = (from order in _query.For(specification)
                           select order.FirmId)
                .ToArray();

            return firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                          .ToArray();
        }
    }
}