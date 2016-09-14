using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Facts
{
    public sealed class BillAccessor : IStorageBasedDataObjectAccessor<Bill>, IDataChangesHandler<Bill>
    {
        private const int BillTypePayment = 1;

        private readonly IQuery _query;

        public BillAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Bill> GetSource()
            => _query.For<Erm::Bill>()
                     .Where(x => x.IsActive && !x.IsDeleted && x.BillType == BillTypePayment)
                     .Select(x => new Bill
                         {
                             Id = x.Id,
                             OrderId = x.OrderId,
                             Begin = x.BeginDistributionDate,
                             End = x.EndDistributionDate,
                             PayablePlan = x.PayablePlan,
                         });

        public FindSpecification<Bill> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Bill>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Bill> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Bill> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Bill> dataObjects)
            => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Bill> dataObjects)
        {
            var orderIds = dataObjects.Select(x => x.OrderId).ToArray();

            return orderIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Order), x)).ToArray();
        }
    }
}