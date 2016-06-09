using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Statistics;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Replication.Actors
{
    public sealed class ProjectStatisticsAggregateRootActor : EntityActorBase<ProjectStatistics>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<ProjectCategoryStatistics> _projectCategoryStatisticsBulkRepository;
        private readonly IBulkRepository<FirmForecast> _firmForecastBulkRepository;
        private readonly IBulkRepository<FirmCategory3> _firmCategory3BulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public ProjectStatisticsAggregateRootActor(
            IQuery query,
            IBulkRepository<ProjectStatistics> projectStatisticsBulkRepository,
            IBulkRepository<ProjectCategoryStatistics> projectCategoryStatisticsBulkRepository,
            IBulkRepository<FirmForecast> firmForecastBulkRepository,
            IBulkRepository<FirmCategory3> firmCategory3BulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, projectStatisticsBulkRepository, equalityComparerFactory, new ProjectStatisticsAccessor(query))
        {
            _query = query;
            _projectCategoryStatisticsBulkRepository = projectCategoryStatisticsBulkRepository;
            _firmForecastBulkRepository = firmForecastBulkRepository;
            _firmCategory3BulkRepository = firmCategory3BulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => new[]
                {
                    new ProjectCategoryStatisticsActor(
                        _query,
                        _projectCategoryStatisticsBulkRepository,
                        _firmCategory3BulkRepository,
                        _equalityComparerFactory)
                };

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new[]
                {
                    new ValueObjectActor<FirmForecast>(_query, _firmForecastBulkRepository, _equalityComparerFactory, new FirmForecastAccessor(_query))
                };

        public sealed class ProjectCategoryStatisticsActor : EntityActorBase<ProjectCategoryStatistics>
        {
            private readonly IQuery _query;
            private readonly IBulkRepository<FirmCategory3> _firmCategory3BulkRepository;
            private readonly IEqualityComparerFactory _equalityComparerFactory;

            public ProjectCategoryStatisticsActor(
                IQuery query,
                IBulkRepository<ProjectCategoryStatistics> bulkRepository,
                IBulkRepository<FirmCategory3> firmCategory3BulkRepository,
                IEqualityComparerFactory equalityComparerFactory)
                : base(query, bulkRepository, equalityComparerFactory, new ProjectCategoryStatisticsAccessor(query))
            {
                _query = query;
                _firmCategory3BulkRepository = firmCategory3BulkRepository;
                _equalityComparerFactory = equalityComparerFactory;
            }

            public override IReadOnlyCollection<IActor> GetValueObjectActors()
                => new[]
                    {
                        new ValueObjectActor<FirmCategory3>(_query, _firmCategory3BulkRepository, _equalityComparerFactory, new FirmCategory3Accessor(_query))
                    };
        }

        public sealed class ProjectStatisticsAccessor : IStorageBasedDataObjectAccessor<ProjectStatistics>
        {
            private readonly IQuery _query;

            public ProjectStatisticsAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<ProjectStatistics> GetSource() => Specs.Map.Facts.ToCI.ProjectStatistics.Map(_query);

            public FindSpecification<ProjectStatistics> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<ProjectStatistics>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class ProjectCategoryStatisticsAccessor : IStorageBasedDataObjectAccessor<ProjectCategoryStatistics>
        {
            private readonly IQuery _query;

            public ProjectCategoryStatisticsAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<ProjectCategoryStatistics> GetSource() => Specs.Map.Facts.ToCI.ProjectCategoryStatistics.Map(_query);

            public FindSpecification<ProjectCategoryStatistics> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return Specs.Find.CI.ProjectCategoryStatistics(aggregateIds);
            }
        }

        public sealed class FirmForecastAccessor : IStorageBasedDataObjectAccessor<FirmForecast>
        {
            private readonly IQuery _query;

            public FirmForecastAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmForecast> GetSource() => Specs.Map.Facts.ToCI.FirmForecast.Map(_query);

            public FindSpecification<FirmForecast> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.CI.FirmForecast(aggregateIds);
            }
        }

        public sealed class FirmCategory3Accessor : IStorageBasedDataObjectAccessor<FirmCategory3>
        {
            private readonly IQuery _query;

            public FirmCategory3Accessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmCategory3> GetSource() => Specs.Map.Facts.ToCI.FirmCategory3.Map(_query);

            public FindSpecification<FirmCategory3> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>()
                                           .Where(c => !c.EntityId.HasValue)
                                           .Select(c => c.AggregateRootId)
                                           .Distinct()
                                           .ToArray();

                var entityIds = commands.OfType<ReplaceValueObjectCommand>()
                                        .Where(c => c.EntityId.HasValue && !aggregateIds.Contains(c.AggregateRootId))
                                        .Select(c => new StatisticsKey { ProjectId = c.AggregateRootId, CategoryId = c.EntityId.Value })
                                        .Distinct()
                                        .ToArray();

                return Specs.Find.CI.FirmCategory3(entityIds) | Specs.Find.CI.FirmCategory3(aggregateIds);
            }
        }
    }
}