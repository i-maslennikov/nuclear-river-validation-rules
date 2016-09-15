using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;

using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Facts;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Facts
{
    public sealed class ProjectAccessor : IStorageBasedDataObjectAccessor<Project>, IDataChangesHandler<Project>
    {
        private readonly IQuery _query;

        public ProjectAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Project> GetSource()
            => _query.For(Specs.Find.Erm.Projects()).Select(x => new Project { Id = x.Id, OrganizationUnitId = x.OrganizationUnitId.Value });

        public FindSpecification<Project> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return new FindSpecification<Project>(x => ids.Contains(x.Id));
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Project> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Project> dataObjects) => Array.Empty<IEvent>();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Project> dataObjects) => Array.Empty<IEvent>();

        // пересчитывать агрегат Order не нужно, т.к. нельзя перепривязать заказ к другому проекту
        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Project> dataObjects) => Array.Empty<IEvent>();
    }
}