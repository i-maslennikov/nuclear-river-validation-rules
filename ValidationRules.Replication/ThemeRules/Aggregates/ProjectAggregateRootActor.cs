using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.ThemeRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ThemeRules.Facts;

namespace NuClear.ValidationRules.Replication.ThemeRules.Aggregates
{
    public sealed class ProjectAggregateRootActor : EntityActorBase<Project>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Project.ProjectDefaultTheme> _projectDefaultThemeBulkRepository;

        public ProjectAggregateRootActor(
            IQuery query,
            IBulkRepository<Project> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Project.ProjectDefaultTheme> projectDefaultThemeBulkRepository)
            : base(query, bulkRepository, equalityComparerFactory, new ProjectAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _projectDefaultThemeBulkRepository = projectDefaultThemeBulkRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Project.ProjectDefaultTheme>(_query, _projectDefaultThemeBulkRepository, _equalityComparerFactory, new ProjectDefaultThemeAccessor(_query)),
                };

        public sealed class ProjectAccessor : IStorageBasedDataObjectAccessor<Project>
        {
            private readonly IQuery _query;

            public ProjectAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Project> GetSource()
                => from project in _query.For<Facts::Project>()
                   select new Project
                   {
                       Id = project.Id,
                       Name = project.Name,
                   };

            public FindSpecification<Project> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Project>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class ProjectDefaultThemeAccessor : IStorageBasedDataObjectAccessor<Project.ProjectDefaultTheme>
        {
            private readonly IQuery _query;

            public ProjectDefaultThemeAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Project.ProjectDefaultTheme> GetSource()
            {
                var themeProjects =
                    from project in _query.For<Facts::Project>()
                    from themeOrgUnit in _query.For<Facts::ThemeOrganizationUnit>().Where(x => x.OrganizationUnitId == project.OrganizationUnitId)
                    from theme in _query.For<Facts::Theme>().Where(x => x.Id == themeOrgUnit.ThemeId)
                    where theme.IsDefault // тематика по умолчанию
                    select new Project.ProjectDefaultTheme
                    {
                        ProjectId = project.Id,
                        ThemeId = theme.Id,
                        Start = theme.BeginDistribution,
                        End = theme.EndDistribution,
                    };

                return themeProjects;
            }

            public FindSpecification<Project.ProjectDefaultTheme> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Project.ProjectDefaultTheme>(x => aggregateIds.Contains(x.ProjectId));
            }
        }
    }
}
