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
using NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Facts
{
    public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>, IDataChangesHandler<Order>
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        private readonly IQuery _query;

        public OrderAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Order> GetSource()
            => from order in _query.For(Specs.Find.Erm.Orders())
               select new Order
                   {
                       Id = order.Id,
                       BeginDistribution = order.BeginDistributionDate,
                       EndDistributionFact = order.EndDistributionDateFact + OneSecond,
                       DestOrganizationUnitId = order.DestOrganizationUnitId,
                       FirmId = order.FirmId,
                       Number = order.Number,
                       WorkflowStep = order.WorkflowStepId,
                   };

        public FindSpecification<Order> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Order>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Order), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Order), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Order), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Order> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var firmIds =
                from order in _query.For<Order>().Where(x => ids.Contains(x.Id))
                from firm in _query.For<Firm>().Where(x => x.Id == order.FirmId)
                select firm.Id;

            return new EventCollectionHelper { { typeof(Firm), firmIds } };
        }
    }
}