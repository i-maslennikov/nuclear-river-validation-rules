using System;
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
    public sealed class CategoryOrganizationUnitAccessor : IStorageBasedDataObjectAccessor<CategoryOrganizationUnit>, IDataChangesHandler<CategoryOrganizationUnit>
    {
        private readonly IQuery _query;

        public CategoryOrganizationUnitAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<CategoryOrganizationUnit> GetSource() => Specs.Map.Erm.ToFacts.CategoryOrganizationUnits.Map(_query);

        public FindSpecification<CategoryOrganizationUnit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<CategoryOrganizationUnit>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<CategoryOrganizationUnit> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<CategoryOrganizationUnit>(x => ids.Contains(x.Id));

            var projectIds = (from categoryOrganizationUnit in _query.For(specification)
                              join project in _query.For<Project>() on categoryOrganizationUnit.OrganizationUnitId equals project.OrganizationUnitId
                              select project.Id)
                .Distinct()
                .ToArray();

            var firmIds = (from firm in _query.For<Firm>()
                           join firmAddress in _query.For<FirmAddress>() on firm.Id equals firmAddress.FirmId
                           join categoryFirmAddress in _query.For<CategoryFirmAddress>() on firmAddress.Id equals categoryFirmAddress.FirmAddressId
                           join categoryOrganizationUnit in _query.For(specification)
                               on categoryFirmAddress.CategoryId equals categoryOrganizationUnit.CategoryId
                           where firm.OrganizationUnitId == categoryOrganizationUnit.OrganizationUnitId
                           select firmAddress.FirmId)
                .Distinct()
                .ToArray();

            var clientIds = (from categoryOrganizationUnit in _query.For(specification)
                             join categoryFirmAddress in _query.For<CategoryFirmAddress>() on categoryOrganizationUnit.CategoryId equals categoryFirmAddress.CategoryId
                             join firmAddress in _query.For<FirmAddress>() on categoryFirmAddress.FirmAddressId equals firmAddress.Id
                             join firm in _query.For<Firm>()
                                 on new { categoryOrganizationUnit.OrganizationUnitId, firmAddress.FirmId } equals new { firm.OrganizationUnitId, FirmId = firm.Id }
                             where firm.ClientId.HasValue
                             select firm.ClientId.Value)
                .ToArray();

            return projectIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Project), x))
                             .Concat(firmIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x)))
                             .Concat(clientIds.Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Client), x)))
                             .ToArray();
        }
    }
}