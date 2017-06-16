using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>, IDataChangesHandler<Order>
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        private readonly IQuery _query;

        public OrderAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Order> GetSource() => _query
            .For(Specs.Find.Erm.Order)
            .Select(order => new Order
            {
                Id = order.Id,
                FirmId = order.FirmId,

                BeginDistribution = order.BeginDistributionDate,
                EndDistributionPlan = order.EndDistributionDatePlan + OneSecond,
                EndDistributionFact = order.EndDistributionDateFact + OneSecond,
                SignupDate = order.SignupDate,

                DestOrganizationUnitId = order.DestOrganizationUnitId,

                LegalPersonId = order.LegalPersonId,
                LegalPersonProfileId = order.LegalPersonProfileId,
                BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId,
                CurrencyId = order.CurrencyId,
                BargainId = order.BargainId,
                DealId = order.DealId,

                WorkflowStep = order.WorkflowStepId,
                IsFreeOfCharge = Erm::Order.FreeOfChargeTypes.Contains(order.OrderType),
                IsSelfAds = order.OrderType == Erm::Order.OrderTypeSelfAds,
            });

        public FindSpecification<Order> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<Order>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Order), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Order), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Order> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Order), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Order> dataObjects)
        {
            var orderIds = dataObjects.Select(x => x.Id).ToList();

            var accountIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                from account in _query.For<Account>().Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                select account.Id;

            var firmIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                from firm in _query.For<Firm>().Where(x => x.Id == order.FirmId)
                select firm.Id;

            var orders =
                (from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                 select new { order.BeginDistribution, order.EndDistributionFact, order.EndDistributionPlan })
                .ToList();

            var periods =
                orders.Select(x => new PeriodKey { Date = x.BeginDistribution })
                      .Concat(orders.Select(x => new PeriodKey { Date = x.EndDistributionFact }))
                      .Concat(orders.Select(x => new PeriodKey { Date = x.EndDistributionPlan }));

            return new EventCollectionHelper<Order> { { typeof(Account), accountIds }, { typeof(Firm), firmIds }, { typeof(object), periods } };
        }
    }
}