using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.CI;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Actors
{
    public sealed class ProjectAggregateRootActor : EntityActorBase<Project>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<ProjectCategory> _projectCategoryBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public ProjectAggregateRootActor(
            IQuery query,
            IBulkRepository<Project> projectBulkRepository,
            IBulkRepository<ProjectCategory> projectCategoryBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
             : base(query, projectBulkRepository, equalityComparerFactory, new ProjectAccessor(query))

        {
            _query = query;
            _projectCategoryBulkRepository = projectCategoryBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors() => new IActor[]
            {
                new ValueObjectActor<ProjectCategory>(_query, _projectCategoryBulkRepository, _equalityComparerFactory, new ProjectCategoryAccessor(_query))
            };

        public sealed class ProjectAccessor : IStorageBasedDataObjectAccessor<Project>
        {
            private readonly IQuery _query;

            public ProjectAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Project> GetSource() => Specs.Map.Facts.ToCI.Projects.Map(_query);

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

        public sealed class ProjectCategoryAccessor : IStorageBasedDataObjectAccessor<ProjectCategory>
        {
            private readonly IQuery _query;

            public ProjectCategoryAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<ProjectCategory> GetSource() => Specs.Map.Facts.ToCI.ProjectCategories.Map(_query);

            public FindSpecification<ProjectCategory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.CI.ProjectCategories(aggregateIds);
            }
        }
    }
}