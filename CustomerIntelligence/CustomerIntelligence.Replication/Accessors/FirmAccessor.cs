using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Bit;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>, IDataChangesHandler<Firm>
    {
        private readonly IQuery _query;

        public FirmAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Firm> GetSource() => Specs.Map.Erm.ToFacts.Firms.Map(_query);

        public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Firm>(x => ids.Contains(x.Id));
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
            var specification = new FindSpecification<Firm>(x => ids.Contains(x.Id));

            var complexIds = (from firm in _query.For(specification)
                              join project in _query.For<Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                              join firmAddress in _query.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                              join firmAddressCategory in _query.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                              select new StatisticsKey { ProjectId = project.Id, CategoryId = firmAddressCategory.CategoryId })
                .ToArray();

            var clientIds = (from firm in _query.For(specification)
                             where firm.ClientId != null
                             select firm.ClientId.Value)
                .ToArray();

            return Enumerable.Empty<IEvent>()
                             .Concat(complexIds.Select(x => new RelatedDataObjectOutdatedEvent<StatisticsKey>(typeof(ProjectCategoryStatistics), x)))
                             .Concat(clientIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x)))
                             .ToArray();
        }
    }
}