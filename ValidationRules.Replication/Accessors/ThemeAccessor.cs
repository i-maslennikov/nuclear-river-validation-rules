using System;
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
    public sealed class ThemeAccessor : IStorageBasedDataObjectAccessor<Theme>, IDataChangesHandler<Theme>
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        private readonly IQuery _query;

        public ThemeAccessor(IQuery query)
        {
            _query = query;
        }

        public IQueryable<Theme> GetSource() => _query
            .For<Erm::Theme>()
            .Where(x => x.IsActive && !x.IsDeleted)
            .Select(x => new Theme
            {
                Id = x.Id,
                Name = x.Name,
                BeginDistribution = x.BeginDistribution,
                EndDistribution = x.EndDistribution + OneSecond,
                IsDefault = x.IsDefault,
            });

        public FindSpecification<Theme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
        {
            var ids = commands.Cast<SyncDataObjectCommand>().Select(c => c.DataObjectId).ToList();
            return SpecificationFactory<Theme>.Contains(x => x.Id, ids);
        }

        public IReadOnlyCollection<IEvent> HandleCreates(IReadOnlyCollection<Theme> dataObjects)
            => dataObjects.Select(x => new DataObjectCreatedEvent(typeof(Theme), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleUpdates(IReadOnlyCollection<Theme> dataObjects)
            => dataObjects.Select(x => new DataObjectUpdatedEvent(typeof(Theme), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleDeletes(IReadOnlyCollection<Theme> dataObjects)
            => dataObjects.Select(x => new DataObjectDeletedEvent(typeof(Theme), x.Id)).ToList();

        public IReadOnlyCollection<IEvent> HandleRelates(IReadOnlyCollection<Theme> dataObjects)
        {
            var dataObjectIds = dataObjects.Select(x => x.Id).ToList();

            var projectIds =
                from themeOrgUnit in _query.For<ThemeOrganizationUnit>().Where(x => dataObjectIds.Contains(x.ThemeId))
                from project in _query.For<Project>().Where(x => x.OrganizationUnitId == themeOrgUnit.OrganizationUnitId)
                select project.Id;

            return new EventCollectionHelper<Theme> { { typeof(Project), projectIds } };
        }
    }
}