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

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<BranchOfficeOrganizationUnit> dataObjects) => Array.Empty<IEvent>();

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