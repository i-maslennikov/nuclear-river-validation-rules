﻿using System.Collections.Generic;
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
    public sealed class ProjectAccessor : IStorageBasedDataObjectAccessor<Project>, IDataChangesHandler<Project>
    {
        private readonly IQuery _query;

        public ProjectAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Project> GetSource() => _query
                .For<Erm::Project>()
                .Where(x => x.IsActive && x.OrganizationUnitId != null)
                .Select(x => new Project
                {
                    Id = x.Id,
                    OrganizationUnitId = x.OrganizationUnitId.Value,
                    Name = x.DisplayName,
                });

        public FindSpecification<Project> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToArray();
            return SpecificationFactory<Project>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Project> dataObjects)
             => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Project), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Project> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Project), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Project> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Project), x.Id)).ToArray();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Project> dataObjects)
        {
            var projectIds = dataObjects.Select(x => x.Id).ToArray();

            var orderIds =
                from project in _query.For<Project>().Where(x => projectIds.Contains(x.Id))
                from order in _query.For<Order>().Where(x => x.DestOrganizationUnitId == project.OrganizationUnitId)
                select order.Id;

            var firmIds =
                from project in _query.For<Project>().Where(x => projectIds.Contains(x.Id))
                from firm in _query.For<Firm>().Where(x => x.OrganizationUnitId == project.OrganizationUnitId)
                select firm.Id;

            return new EventCollectionHelper { { typeof(Order), orderIds }, { typeof(Firm), firmIds } };
        }
    }
}