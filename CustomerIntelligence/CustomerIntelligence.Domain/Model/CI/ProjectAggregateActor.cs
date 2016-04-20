using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class ProjectAggregateActor : IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Project> _projectBulkRepository;
        private readonly IBulkRepository<ProjectCategory> _projectCategoryBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public ProjectAggregateActor(
            IQuery query,
            IBulkRepository<Project> projectBulkRepository,
            IBulkRepository<ProjectCategory> projectCategoryBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
        {
            _query = query;
            _projectBulkRepository = projectBulkRepository;
            _projectCategoryBulkRepository = projectCategoryBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var actor = new ProjectActor(_query, _projectBulkRepository, _equalityComparerFactory);
            return actor.ExecuteCommands(commands);
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();

        public IReadOnlyCollection<IActor> GetValueObjectActors() => new IActor[]
            {
                new ReplaceStorageBasedDataObjectsActor<ProjectCategory>(
                    _query,
                    _projectCategoryBulkRepository,
                    _equalityComparerFactory,
                    new ProjectCategoryAccessor(_query))
            };

        private sealed class ProjectActor : AggregateRootActorBase<Project>
        {
            public ProjectActor(IQuery query,
                               IBulkRepository<Project> projectBulkRepository,
                               IEqualityComparerFactory equalityComparerFactory)
                : base(query, projectBulkRepository, equalityComparerFactory, new ProjectAccessor(query))
            {
            }
        }

        public sealed class ProjectAccessor : IStorageBasedDataObjectAccessor<Project>
        {
            private readonly IQuery _query;

            public ProjectAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Project> GetSource() => Specs.Map.Facts.ToCI.Projects.Map(_query);

            public FindSpecification<Project> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => new FindSpecification<Project>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().Contains(x.Id));
        }

        public sealed class ProjectCategoryAccessor : IStorageBasedDataObjectAccessor<ProjectCategory>
        {
            private readonly IQuery _query;

            public ProjectCategoryAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<ProjectCategory> GetSource() => Specs.Map.Facts.ToCI.ProjectCategories.Map(_query);

            public FindSpecification<ProjectCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.ProjectCategories(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }
    }
}