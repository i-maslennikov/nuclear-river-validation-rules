using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Specs;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Events;
using NuClear.ValidationRules.Storage.Model.Facts;

using Erm = NuClear.ValidationRules.Storage.Model.Erm;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>, IDataChangesHandler<Firm>
    {
        private readonly IQuery _query;

        public FirmAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Firm> GetSource() => _query
            .For<Erm::Firm>()
            .Select(x => new Firm
            {
                Id = x.Id,
                OrganizationUnitId = x.OrganizationUnitId,
                Name = x.Name,

                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                IsClosedForAscertainment = x.ClosedForAscertainment,
            });

        public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return SpecificationFactory<Firm>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Firm> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from order in _query.For<Order>().Where(x => ids.Contains(x.FirmId))
                select order.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds } };
        }
    }
}