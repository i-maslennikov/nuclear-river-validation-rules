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
using NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Facts
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
            => from x in _query.For(Specs.Find.Erm.Orders())
               select new Order
                   {
                       Id = x.Id,
                       BeginDistribution = x.BeginDistributionDate,
                       EndDistributionPlan = x.EndDistributionDatePlan + OneSecond,
                       DestOrganizationUnitId = x.DestOrganizationUnitId,
                       Number = x.Number,
                       WorkflowStep = x.WorkflowStepId,
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
            => Array.Empty<IEvent>();
    }
}