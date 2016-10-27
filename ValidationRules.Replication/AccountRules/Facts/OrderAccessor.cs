﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.AccountRules.Facts;

namespace NuClear.ValidationRules.Replication.AccountRules.Facts
{
    public sealed class OrderAccessor : IStorageBasedDataObjectAccessor<Order>, IDataChangesHandler<Order>
    {
        private const int OrderOnTermination = 4;
        private const int OrderApproved = 5;
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);
        private static readonly int[] FreeOfChargeOrderTypes = { 2, 7, 9 };

        private readonly IQuery _query;

        public OrderAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Order> GetSource()
            => _query.For(Specs.Find.Erm.Orders())
                     .Where(x => x.WorkflowStepId == OrderOnTermination || x.WorkflowStepId == OrderApproved)
                     .Select(order => new Order
                         {
                             Id = order.Id,
                             DestOrganizationUnitId = order.DestOrganizationUnitId,
                             SourceOrganizationUnitId = order.SourceOrganizationUnitId,
                             BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId,
                             LegalPersonId = order.LegalPersonId,
                             IsFreeOfCharge = FreeOfChargeOrderTypes.Contains(order.OrderType),
                             Number = order.Number,
                             BeginDistributionDate = order.BeginDistributionDate,
                             EndDistributionDate = order.EndDistributionDateFact + OneSecond,
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

            return new EventCollectionHelper { { typeof(Account), accountIds.Distinct() } };
        }
    }
}