using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>, IDataChangesHandler<Order>
    {
        private const int OrderStateArchive = 6;
        private const int OrderStateRejected = 3;
        private const int OrderTypeSelfAds = 2;
        private const int OrderTypeSocialAds = 7;
        private const int OrderTypeCompensation = 9;

        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        private readonly IQuery _query;

        public OrderAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Order> GetSource() => _query
            .For<Erm::Order>()
            .Where(x => x.IsActive && !x.IsDeleted && x.WorkflowStepId != OrderStateArchive && x.WorkflowStepId != OrderStateRejected)
            .Select(order => new Order
            {
                Id = order.Id,
                FirmId = order.FirmId,
                Number = order.Number,

                BeginDistribution = order.BeginDistributionDate,
                EndDistributionPlan = order.EndDistributionDatePlan + OneSecond,
                EndDistributionFact = order.EndDistributionDateFact + OneSecond,
                SignupDate = order.SignupDate,

                DestOrganizationUnitId = order.DestOrganizationUnitId,

                LegalPersonId = order.LegalPersonId,
                LegalPersonProfileId = order.LegalPersonProfileId,
                BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId,
                InspectorId = order.InspectorCode,
                CurrencyId = order.CurrencyId,
                BargainId = order.BargainId,
                DealId = order.DealId,

                WorkflowStep = order.WorkflowStepId,
                IsFreeOfCharge = order.OrderType == OrderTypeSelfAds || order.OrderType == OrderTypeSocialAds || order.OrderType == OrderTypeCompensation,
                IsSelfAds = order.OrderType == OrderTypeSelfAds,

                ReleaseCountPlan = order.ReleaseCountPlan,
            });

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
            var orderIds = dataObjects.Select(x => x.Id).ToArray();

            var accountIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                from account in _query.For<Account>().Where(x => x.LegalPersonId == order.LegalPersonId && x.BranchOfficeOrganizationUnitId == order.BranchOfficeOrganizationUnitId)
                select account.Id;

            var firmIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                from firm in _query.For<Firm>().Where(x => x.Id == order.FirmId)
                select firm.Id;

            // И какой тип я должен тут указать?
            // Тип outdated-сущности - это период. Нет периода в фактах, а агрегатный тип тут указывать некорректно.
            var periodIds =
                from order in _query.For<Order>().Where(x => orderIds.Contains(x.Id))
                group order by order.DestOrganizationUnitId into orders
                select new PeriodKey { OrganizationUnitId = orders.Key, Start = orders.Min(y => y.BeginDistribution), End = orders.Max(y => y.EndDistributionFact) };

            return new EventCollectionHelper { { typeof(Account), accountIds.Distinct() }, { typeof(Firm), firmIds }, { typeof(Order), periodIds } };
        }
    }
}