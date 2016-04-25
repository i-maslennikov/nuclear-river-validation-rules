using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.CustomerIntelligence.Storage.Model.Statistics;
using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Actors
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

        private sealed class ProjectCategoryStatisticsActor : EntityActorBase<ProjectCategoryStatistics>
        {
            private readonly IQuery _query;
            private readonly IBulkRepository<FirmCategory3> _firmCategory3BulkRepository;
            private readonly IEqualityComparerFactory _equalityComparerFactory;


            public ProjectCategoryStatisticsActor(
                IQuery query,
                IBulkRepository<ProjectCategoryStatistics> projectCategoryStatisticsBulkRepository,
                IBulkRepository<FirmCategory3> firmCategory3BulkRepository,
                IEqualityComparerFactory equalityComparerFactory)
                : base(query, projectCategoryStatisticsBulkRepository, equalityComparerFactory, new ProjectCategoryStatisticsAccessor(query))
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

        private sealed class ProjectStatisticsAccessor : IStorageBasedDataObjectAccessor<ProjectStatistics>
        {
            private readonly IQuery _query;

            public ProjectStatisticsAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<ProjectStatistics> GetSource() => Specs.Map.Facts.ToCI.ProjectStatistics.Map(_query);

            public FindSpecification<ProjectStatistics> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                return new FindSpecification<ProjectStatistics>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().Contains(x.Id));
            }
        }

        private sealed class ProjectCategoryStatisticsAccessor : IStorageBasedDataObjectAccessor<ProjectCategoryStatistics>
        {
            private readonly IQuery _query;

            public ProjectCategoryStatisticsAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<ProjectCategoryStatistics> GetSource() => Specs.Map.Facts.ToCI.ProjectCategoryStatistics.Map(_query);

            public FindSpecification<ProjectCategoryStatistics> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.ProjectCategoryStatistics(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        private sealed class FirmForecastAccessor : IStorageBasedDataObjectAccessor<FirmForecast>
        {
            private readonly IQuery _query;

            public FirmForecastAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmForecast> GetSource() => Specs.Map.Facts.ToCI.FirmForecast.Map(_query);

            public FindSpecification<FirmForecast> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmForecast(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        private sealed class FirmCategory3Accessor : IStorageBasedDataObjectAccessor<FirmCategory3>
        {
            private readonly IQuery _query;

            public FirmCategory3Accessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmCategory3> GetSource() => Specs.Map.Facts.ToCI.FirmCategory3.Map(_query);

            public FindSpecification<FirmCategory3> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmCategory3(commands.OfType<RecalculateEntityCommand>()
                                                       .Select(c => new StatisticsKey { ProjectId = c.AggregateRootId, CategoryId = c.EntityId })
                                                       .Distinct()
                                                       .ToArray());
        }
    }
}