using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.ProjectRules.Aggregates
{
    public sealed class ProjectAggregateRootActor : AggregateRootActor<Project>
    {
        public ProjectAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Project> bulkRepository,
            IBulkRepository<Project.Category> categoryRepository,
            IBulkRepository<Project.CostPerClickRestriction> costPerClickRestrictionRepository,
            IBulkRepository<Project.SalesModelRestriction> salesModelRestrictionRepository,
            IBulkRepository<Project.NextRelease> nextReleaseRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new ProjectAccessor(query), bulkRepository,
               HasValueObject(new CategoryAccessor(query), categoryRepository),
               HasValueObject(new CostPerClickRestrictionAccessor(query), costPerClickRestrictionRepository),
               HasValueObject(new SalesModelRestrictionAccessor(query), salesModelRestrictionRepository),
               HasValueObject(new NextReleaseAccessor(query), nextReleaseRepository));
        }

        public sealed class ProjectAccessor : DataChangesHandler<Project>, IStorageBasedDataObjectAccessor<Project>
        {
            private readonly IQuery _query;

            public ProjectAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderMustNotIncludeReleasedPeriod,
                        MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject,
                        MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum,
                        MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel,
                        MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction,
                    };

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

        public sealed class CategoryAccessor : DataChangesHandler<Project.Category>, IStorageBasedDataObjectAccessor<Project.Category>
        {
            private readonly IQuery _query;

            public CategoryAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject,
                    };

            public IQueryable<Project.Category> GetSource()
                => from project in _query.For<Facts::Project>()
                   from link in _query.For<Facts::CategoryOrganizationUnit>().Where(x => x.OrganizationUnitId == project.OrganizationUnitId)
                   select new Project.Category { ProjectId = project.Id, CategoryId = link.CategoryId };

            public FindSpecification<Project.Category> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Project.Category>(x => aggregateIds.Contains(x.ProjectId));
            }
        }

        public sealed class CostPerClickRestrictionAccessor : DataChangesHandler<Project.CostPerClickRestriction>, IStorageBasedDataObjectAccessor<Project.CostPerClickRestriction>
        {
            private readonly IQuery _query;

            public CostPerClickRestrictionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionCostPerClickMustNotBeLessMinimum,
                        MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction,
                    };

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
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Project.CostPerClickRestriction>(x => aggregateIds.Contains(x.ProjectId));
            }
        }

        public sealed class SalesModelRestrictionAccessor : DataChangesHandler<Project.SalesModelRestriction>, IStorageBasedDataObjectAccessor<Project.SalesModelRestriction>
        {
            private readonly IQuery _query;

            public SalesModelRestrictionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel,
                    };

            public IQueryable<Project.SalesModelRestriction> GetSource()
                => from project in _query.For<Facts::Project>()
                   from restriction in _query.For<Facts::SalesModelCategoryRestriction>().Where(x => x.ProjectId == project.Id)
                   let nextRestriction = _query.For<Facts::SalesModelCategoryRestriction>().Where(x => x.ProjectId == project.Id && x.CategoryId == restriction.CategoryId && x.Begin > restriction.Begin).Min(x => (DateTime?)x.Begin)
                   select new Project.SalesModelRestriction
                   {
                       ProjectId = restriction.ProjectId,
                       CategoryId = restriction.CategoryId,
                       SalesModel = restriction.SalesModel,
                       Begin = restriction.Begin,
                       End = nextRestriction ?? DateTime.MaxValue
                   };

            public FindSpecification<Project.SalesModelRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Project.SalesModelRestriction>(x => aggregateIds.Contains(x.ProjectId));
            }
        }

        public sealed class NextReleaseAccessor : DataChangesHandler<Project.NextRelease>, IStorageBasedDataObjectAccessor<Project.NextRelease>
        {
            private readonly IQuery _query;

            public NextReleaseAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.OrderMustNotIncludeReleasedPeriod,
                    };

            public IQueryable<Project.NextRelease> GetSource()
                => from project in _query.For<Facts::Project>()
                   from releaseInfo in _query.For<Facts::ReleaseInfo>().Where(x => x.OrganizationUnitId == project.OrganizationUnitId).OrderByDescending(x => x.PeriodEndDate).Take(1)
                   select new Project.NextRelease
                   {
                       ProjectId = project.Id,
                       Date = releaseInfo.PeriodEndDate,
                   };

            public FindSpecification<Project.NextRelease> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Project.NextRelease>(x => aggregateIds.Contains(x.ProjectId));
            }
        }

    }
}
