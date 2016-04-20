using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Events;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Facts
{
    public sealed class BranchOfficeOrganizationUnitAccessor : IStorageBasedDataObjectAccessor<BranchOfficeOrganizationUnit>, IDataChangesHandler<BranchOfficeOrganizationUnit>
    {
        private readonly IQuery _query;

        public BranchOfficeOrganizationUnitAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<BranchOfficeOrganizationUnit> GetSource() => Specs.Map.Erm.ToFacts.BranchOfficeOrganizationUnits.Map(_query);

        public FindSpecification<BranchOfficeOrganizationUnit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            => new FindSpecification<BranchOfficeOrganizationUnit>(x => commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).Contains(x.Id));

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(BranchOfficeOrganizationUnit), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(BranchOfficeOrganizationUnit), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(BranchOfficeOrganizationUnit), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects)
        {
            var ids = dataObjects.Select(x => x.Id).ToArray();
            var specification = new FindSpecification<BranchOfficeOrganizationUnit>(x => ids.Contains(x.Id));

            return Specs.Map.Facts.ToFirmAggregate.ByBranchOfficeOrganizationUnit(specification)
                        .Map(_query)
                        .Select(x => new RelatedDataObjectOutdatedEvent<long>(typeof(Firm), x))
                        .ToArray();
        }
    }
}