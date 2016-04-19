using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata;
using NuClear.River.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.Statistics
{
    public class ProjectStatisticsAggregateActor : IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<ProjectStatistics> _projectStatisticsBulkRepository;
        private readonly IBulkRepository<ProjectCategoryStatistics> _projectCategoryStatisticsBulkRepository;
        private readonly IBulkRepository<FirmForecast> _firmForecastBulkRepository;
        private readonly IBulkRepository<FirmCategory3> _firmCategory3BulkRepository;

        public ProjectStatisticsAggregateActor(
            IQuery query,
            IBulkRepository<ProjectStatistics> projectStatisticsBulkRepository,
            IBulkRepository<ProjectCategoryStatistics> projectCategoryStatisticsBulkRepository,
            IBulkRepository<FirmForecast> firmForecastBulkRepository,
            IBulkRepository<FirmCategory3> firmCategory3BulkRepository)
        {
            _query = query;
            _projectStatisticsBulkRepository = projectStatisticsBulkRepository;
            _projectCategoryStatisticsBulkRepository = projectCategoryStatisticsBulkRepository;
            _firmForecastBulkRepository = firmForecastBulkRepository;
            _firmCategory3BulkRepository = firmCategory3BulkRepository;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var actor = new ProjectStatisticsActor(_query, _projectStatisticsBulkRepository);
            return actor.ExecuteCommands(commands);
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors()
            => new[] { new ProjectCategoryStatisticsActor(_query, _projectCategoryStatisticsBulkRepository, _firmCategory3BulkRepository) };

        public IReadOnlyCollection<IActor> GetValueObjectActors()
            => new[] { new ReplaceStorageBasedDataObjectsActor<FirmForecast>(_query, _firmForecastBulkRepository, new FirmForecastAccessor(_query)) };

        private class ProjectStatisticsActor : AggregateRootActorBase<ProjectStatistics>
        {
            public ProjectStatisticsActor(IQuery query, IBulkRepository<ProjectStatistics> projectCategoryStatisticsBulkRepository)
                : base(query, projectCategoryStatisticsBulkRepository, new ProjectStatisticsAccessor(query))
            {
            }
        }

        private class ProjectCategoryStatisticsActor : IEntityActor
        {
            private readonly IQuery _query;
            private readonly IBulkRepository<ProjectCategoryStatistics> _projectCategoryStatisticsBulkRepository;
            private readonly IBulkRepository<FirmCategory3> _firmCategory3BulkRepository;

            public ProjectCategoryStatisticsActor(
                IQuery query,
                IBulkRepository<ProjectCategoryStatistics> projectCategoryStatisticsBulkRepository,
                IBulkRepository<FirmCategory3> firmCategory3BulkRepository)
            {
                _query = query;
                _projectCategoryStatisticsBulkRepository = projectCategoryStatisticsBulkRepository;
                _firmCategory3BulkRepository = firmCategory3BulkRepository;
            }

            public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
            {
                var actor = new SyncDataObjectsActor<ProjectCategoryStatistics>(_query, _projectCategoryStatisticsBulkRepository, new ProjectCategoryStatisticsAccessor(_query));
                return actor.ExecuteCommands(commands);
            }

            public IReadOnlyCollection<IActor> GetValueObjectActors()
                => new [] { new ReplaceStorageBasedDataObjectsActor<FirmCategory3>(_query, _firmCategory3BulkRepository, new FirmCategory3Accessor(_query))};
        }

        private class ProjectStatisticsAccessor : IStorageBasedDataObjectAccessor<ProjectStatistics>
        {
            private readonly IQuery _query;

            public ProjectStatisticsAccessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<ProjectStatistics> EqualityComparer => null;

            public IQueryable<ProjectStatistics> GetSource() => Specs.Map.Facts.ToCI.ProjectStatistics.Map(_query);

            public FindSpecification<ProjectStatistics> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                return new FindSpecification<ProjectStatistics>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().Contains(x.Id));
            }
        }

        private class ProjectCategoryStatisticsAccessor : IStorageBasedDataObjectAccessor<ProjectCategoryStatistics>
        {
            private readonly IQuery _query;

            public ProjectCategoryStatisticsAccessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<ProjectCategoryStatistics> EqualityComparer => null;

            public IQueryable<ProjectCategoryStatistics> GetSource() => Specs.Map.Facts.ToCI.ProjectCategoryStatistics.Map(_query);

            public FindSpecification<ProjectCategoryStatistics> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.ProjectCategoryStatistics(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        private class FirmForecastAccessor : IStorageBasedDataObjectAccessor<FirmForecast>
        {
            private readonly IQuery _query;

            public FirmForecastAccessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<FirmForecast> EqualityComparer => null;

            public IQueryable<FirmForecast> GetSource() => Specs.Map.Facts.ToCI.FirmForecast.Map(_query);

            public FindSpecification<FirmForecast> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmForecast(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        private class FirmCategory3Accessor : IStorageBasedDataObjectAccessor<FirmCategory3>
        {
            private readonly IQuery _query;

            public FirmCategory3Accessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<FirmCategory3> EqualityComparer => null;

            public IQueryable<FirmCategory3> GetSource() => Specs.Map.Facts.ToCI.FirmCategory3.Map(_query);

            public FindSpecification<FirmCategory3> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            /*
                => new FindSpecification<FirmCategory3>(x => commands.Cast<IAggregateCommand>()
                                                                .OfType<RecalculateEntityCommand>()
                                                                .Distinct()
                                                                .Any(c => c.AggregateRootId == x.ProjectId && c.EntityId == x.CategoryId));
            */
                => new FindSpecification<FirmCategory3>(x => commands.Cast<IAggregateCommand>()
                                                                     .OfType<RecalculateEntityCommand>()
                                                                     .Select(c => new StatisticsKey { ProjectId = c.AggregateRootId, CategoryId = c.EntityId })
                                                                     .Distinct()
                                                                     .ToArray()
                                                                     .Contains(new StatisticsKey { ProjectId = x.ProjectId, CategoryId = x.CategoryId }));
        }
    }
}