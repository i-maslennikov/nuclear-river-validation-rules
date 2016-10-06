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
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.ProjectRules.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Aggregates
{
    public sealed class ProjectAggregateRootActor : EntityActorBase<Project>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IEqualityComparerFactory _equalityComparerFactory;
        private readonly IBulkRepository<Project.Category> _categoryRepository;
        private readonly IBulkRepository<Project.CostPerClickRestriction> _costPerClickRestrictionRepository;
        private readonly IBulkRepository<Project.NextRelease> _nextReleaseRepository;

        public ProjectAggregateRootActor(
            IQuery query,
            IBulkRepository<Project> bulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Project.Category> categoryRepository,
            IBulkRepository<Project.CostPerClickRestriction> costPerClickRestrictionRepository,
            IBulkRepository<Project.NextRelease> nextReleaseRepository)
            : base(query, bulkRepository, equalityComparerFactory, new ProjectAccessor(query))
        {
            _query = query;
            _equalityComparerFactory = equalityComparerFactory;
            _categoryRepository = categoryRepository;
            _costPerClickRestrictionRepository = costPerClickRestrictionRepository;
            _nextReleaseRepository = nextReleaseRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<Project.Category>(_query, _categoryRepository, _equalityComparerFactory, new CategoryAccessor(_query)),
                    new ValueObjectActor<Project.CostPerClickRestriction>(_query, _costPerClickRestrictionRepository, _equalityComparerFactory, new CostPerClickRestrictionAccessor(_query)),
                    new ValueObjectActor<Project.NextRelease>(_query, _nextReleaseRepository, _equalityComparerFactory, new NextReleaseAccessor(_query)),
                };

        public sealed class ProjectAccessor : IStorageBasedDataObjectAccessor<Project>
        {
            private readonly IQuery _query;

            public ProjectAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Project> GetSource()
                => from category in _query.For<Facts::Project>()
                   select new Project { Id = category.Id, Name = category.Name };

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

        public sealed class CategoryAccessor : IStorageBasedDataObjectAccessor<Project.Category>
        {
            private readonly IQuery _query;

            public CategoryAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Project.Category> GetSource()
                => from project in _query.For<Facts::Project>()
                   from link in _query.For<Facts::CategoryOrganizationUnit>().Where(x => x.OrganizationUnitId == project.OrganizationUnitId)
                   select new Project.Category { ProjectId = project.Id, CategoryId = link.CategoryId };

            public FindSpecification<Project.Category> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Project.Category>(x => aggregateIds.Contains(x.ProjectId));
            }
        }

        public sealed class CostPerClickRestrictionAccessor : IStorageBasedDataObjectAccessor<Project.CostPerClickRestriction>
        {
            private readonly IQuery _query;

            public CostPerClickRestrictionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Project.CostPerClickRestriction> GetSource()
                => from project in _query.For<Facts::Project>()
                   from restriction in _query.For<Facts::CostPerClickCategoryRestriction>().Where(x => x.ProjectId == project.Id)
                   let nextRestriction = _query.For<Facts::CostPerClickCategoryRestriction>().Where(x => x.ProjectId == project.Id && x.CategoryId == restriction.CategoryId && x.Begin > restriction.Begin).Min(x => (DateTime?)x.Begin)
                   select new Project.CostPerClickRestriction
                   {
                       ProjectId = restriction.ProjectId,
                       CategoryId = restriction.CategoryId,
                       Minimum = restriction.MinCostPerClick,
                       Begin = restriction.Begin,
                       End = nextRestriction ?? DateTime.MaxValue
                   };

            public FindSpecification<Project.CostPerClickRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Project.CostPerClickRestriction>(x => aggregateIds.Contains(x.ProjectId));
            }
        }

        public sealed class NextReleaseAccessor : IStorageBasedDataObjectAccessor<Project.NextRelease>
        {
            private readonly IQuery _query;

            public NextReleaseAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Project.NextRelease> GetSource()
                => from project in _query.For<Facts::Project>()
                   from releaseInfo in _query.For<Facts::ReleaseInfo>().Where(x => x.OrganizationUnitId == project.OrganizationUnitId)
                   select new Project.NextRelease
                   {
                       ProjectId = project.Id,
                       Date = releaseInfo.PeriodEndDate,
                   };

            public FindSpecification<Project.NextRelease> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Project.NextRelease>(x => aggregateIds.Contains(x.ProjectId));
            }
        }

    }
}
