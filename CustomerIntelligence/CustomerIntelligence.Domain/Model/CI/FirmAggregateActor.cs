using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API;
using NuClear.Replication.Core.API.Aggregates;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Model.CI
{
    public sealed class FirmAggregateActor : IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<Firm> _firmBulkRepository;
        private readonly IBulkRepository<FirmActivity> _firmActivityBulkRepository;
        private readonly IBulkRepository<FirmBalance> _firmBalanceBulkRepository;
        private readonly IBulkRepository<FirmCategory1> _firmCategory1BulkRepository;
        private readonly IBulkRepository<FirmCategory2> _firmCategory2BulkRepository;
        private readonly IBulkRepository<FirmTerritory> _firmFirmTerritoryBulkRepository;

        public FirmAggregateActor(
            IQuery query,
            IBulkRepository<Firm> firmBulkRepository,
            IBulkRepository<FirmActivity> firmActivityBulkRepository,
            IBulkRepository<FirmBalance> firmBalanceBulkRepository,
            IBulkRepository<FirmCategory1> firmCategory1BulkRepository,
            IBulkRepository<FirmCategory2> firmCategory2BulkRepository,
            IBulkRepository<FirmTerritory> firmFirmTerritoryBulkRepository)
        {
            _query = query;
            _firmBulkRepository = firmBulkRepository;
            _firmActivityBulkRepository = firmActivityBulkRepository;
            _firmBalanceBulkRepository = firmBalanceBulkRepository;
            _firmCategory1BulkRepository = firmCategory1BulkRepository;
            _firmCategory2BulkRepository = firmCategory2BulkRepository;
            _firmFirmTerritoryBulkRepository = firmFirmTerritoryBulkRepository;
        }

        public IReadOnlyCollection<IEvent> ExecuteCommands(IReadOnlyCollection<ICommand> commands)
        {
            var actor = new FirmActor(_query, _firmBulkRepository);
            return actor.ExecuteCommands(commands);
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

        public IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ReplaceStorageBasedDataObjectsActor<FirmActivity>(_query, _firmActivityBulkRepository, new FirmActivityAccessor(_query)),
                    new ReplaceStorageBasedDataObjectsActor<FirmBalance>(_query, _firmBalanceBulkRepository, new FirmBalanceAccessor(_query)),
                    new ReplaceStorageBasedDataObjectsActor<FirmCategory1>(_query, _firmCategory1BulkRepository, new FirmCategory1Accessor(_query)),
                    new ReplaceStorageBasedDataObjectsActor<FirmCategory2>(_query, _firmCategory2BulkRepository, new FirmCategory2Accessor(_query)),
                    new ReplaceStorageBasedDataObjectsActor<FirmTerritory>(_query, _firmFirmTerritoryBulkRepository, new FirmTerritoryAccessor(_query))
                };

        private class FirmActor : AggregateRootActorBase<Firm>
        {
            public FirmActor(IQuery query, IBulkRepository<Firm> projectCategoryStatisticsBulkRepository)
                : base(query, projectCategoryStatisticsBulkRepository, new FirmAccessor(query))
            {
            }
        }

        private class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>
        {
            private readonly IQuery _query;

            public FirmAccessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<Firm> EqualityComparer => null;

            public IQueryable<Firm> GetSource() => Specs.Map.Facts.ToCI.Firms.Map(_query);

            public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => new FindSpecification<Firm>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().Contains(x.Id));
        }

        private class FirmActivityAccessor : IStorageBasedDataObjectAccessor<FirmActivity>
        {
            private readonly IQuery _query;

            public FirmActivityAccessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<FirmActivity> EqualityComparer => null;

            public IQueryable<FirmActivity> GetSource() => Specs.Map.Facts.ToCI.FirmActivities.Map(_query);

            public FindSpecification<FirmActivity> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmActivities(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        private class FirmBalanceAccessor : IStorageBasedDataObjectAccessor<FirmBalance>
        {
            private readonly IQuery _query;

            public FirmBalanceAccessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<FirmBalance> EqualityComparer => null;

            public IQueryable<FirmBalance> GetSource() => Specs.Map.Facts.ToCI.FirmBalances.Map(_query);

            public FindSpecification<FirmBalance> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmBalances(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        private class FirmCategory1Accessor : IStorageBasedDataObjectAccessor<FirmCategory1>
        {
            private readonly IQuery _query;

            public FirmCategory1Accessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<FirmCategory1> EqualityComparer => null;

            public IQueryable<FirmCategory1> GetSource() => Specs.Map.Facts.ToCI.FirmCategories1.Map(_query);

            public FindSpecification<FirmCategory1> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmCategories1(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        private class FirmCategory2Accessor : IStorageBasedDataObjectAccessor<FirmCategory2>
        {
            private readonly IQuery _query;

            public FirmCategory2Accessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<FirmCategory2> EqualityComparer => null;

            public IQueryable<FirmCategory2> GetSource() => Specs.Map.Facts.ToCI.FirmCategories2.Map(_query);

            public FindSpecification<FirmCategory2> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmCategories2(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        private class FirmTerritoryAccessor : IStorageBasedDataObjectAccessor<FirmTerritory>
        {
            private readonly IQuery _query;

            public FirmTerritoryAccessor(IQuery query)
            {
                _query = query;
            }

            public IEqualityComparer<FirmTerritory> EqualityComparer => null;

            public IQueryable<FirmTerritory> GetSource() => Specs.Map.Facts.ToCI.FirmTerritories.Map(_query);

            public FindSpecification<FirmTerritory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmTerritories(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }
    }
}