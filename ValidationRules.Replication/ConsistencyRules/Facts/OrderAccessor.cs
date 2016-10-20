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
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;


namespace NuClear.ValidationRules.Replication.ConsistencyRules.Facts
{
    public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>, IDataChangesHandler<Order>
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        private static readonly int[] FreeOfChargeOrderTypes = { 2, 7, 9 };

        private readonly IQuery _query;

        public OrderAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Order> GetSource()
            => _query.For(Specs.Find.Erm.Orders())
                     .Select(order => new Order
                         {
                             Id = order.Id,
                             FirmId = order.FirmId,
                             DestOrganizationUnitId = order.DestOrganizationUnitId,
                             LegalPersonId = order.LegalPersonId,
                             LegalPersonProfileId = order.LegalPersonProfileId,
                             BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId,
                             InspectorId = order.InspectorCode,
                             CurrencyId = order.CurrencyId,
                             BargainId = order.BargainId,
                             DealId = order.DealId,
                             WorkflowStep = order.WorkflowStepId,
                             IsFreeOfCharge = FreeOfChargeOrderTypes.Contains(order.OrderType),

                             SignupDate = order.SignupDate,
                             BeginDistribution = order.BeginDistributionDate,
                             EndDistributionFact = order.EndDistributionDateFact + OneSecond,
                             EndDistributionPlan = order.EndDistributionDatePlan + OneSecond,
                             ReleaseCountPlan = order.ReleaseCountPlan,
                             Number = order.Number,
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
            => Array.Empty<IEvent>();
    }
}