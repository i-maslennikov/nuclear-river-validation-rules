using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Accessors
{
    public sealed class OrganizationUnitAccessor : IStorageBasedDataObjectAccessor<OrganizationUnit>, IDataChangesHandler<OrganizationUnit>
    {
        private readonly IQuery _query;

        public OrganizationUnitAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<OrganizationUnit> GetSource() => Specs.Map.Erm.ToFacts.OrganizationUnit.Map(_query);

        public FindSpecification<OrganizationUnit> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<OrganizationUnit>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<OrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<OrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<OrganizationUnit> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<OrganizationUnit> dataObjects) => Array.Empty<IEvent>();
    }
}