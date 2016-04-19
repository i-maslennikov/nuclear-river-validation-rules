using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Model.Bit;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
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
            => new FindSpecification<Firm>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Firm> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<Firm>(x => ids.Contains(x.Id));

            IEnumerable<IEvent> events = from firm in _query.For(specification)
                                         join project in _query.For<Project>() on firm.OrganizationUnitId equals project.OrganizationUnitId
                                         join firmAddress in _query.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                                         join firmAddressCategory in _query.For<CategoryFirmAddress>() on firmAddress.Id equals firmAddressCategory.FirmAddressId
                                         select new RelatedDataObjectOutdatedEvent<StatisticsKey>(
                                             typeof(ProjectCategoryStatistics),
                                             new StatisticsKey { ProjectId = project.Id, CategoryId = firmAddressCategory.CategoryId });

            events = events.Concat(from firm in _query.For(specification)
                                   where firm.ClientId != null
                                   select new RelatedDataObjectOutdatedEvent<long>(typeof(Client), firm.ClientId.Value));
            return events.ToArray();
        }

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Firm), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Firm> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Firm), x.Id)).ToArray();
    }
}