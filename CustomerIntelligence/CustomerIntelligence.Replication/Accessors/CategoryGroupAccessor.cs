using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Events;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Facts;
using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Accessors
{
    public sealed class CategoryGroupAccessor : IStorageBasedDataObjectAccessor<CategoryGroup>, IDataChangesHandler<CategoryGroup>
    {
        private readonly IQuery _query;

        public CategoryGroupAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CategoryGroup> GetSource() => Specs.Map.Erm.ToFacts.CategoryGroups.Map(_query);

        public FindSpecification<CategoryGroup> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<CategoryGroup>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CategoryGroup> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(CategoryGroup), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CategoryGroup> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(CategoryGroup), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CategoryGroup> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(CategoryGroup), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CategoryGroup> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<CategoryGroup>(x => ids.Contains(x.Id));

            var firmIds = (from categoryGroup in _query.For(specification)
                           join categoryOrganizationUnit in _query.For<CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                           join categoryFirmAddress in _query.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                           join firmAddress in _query.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                           select firmAddress.FirmId)
                .Distinct()
                .ToArray();

            var clientIds = (from categoryGroup in _query.For(specification)
                             join categoryOrganizationUnit in _query.For<CategoryOrganizationUnit>() on categoryGroup.Id equals categoryOrganizationUnit.CategoryId
                             join categoryFirmAddress in _query.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                             join firmAddress in _query.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                             join firm in _query.For<Firm>()
                                 on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals new { firm.OrganizationUnitId, FirmId = firm.Id }
                             where firm.ClientId.HasValue
                             select firm.ClientId.Value)
                .Distinct()
                .ToArray();

            return firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                          .Concat(clientIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x)))
                          .ToArray();
        }
    }
}