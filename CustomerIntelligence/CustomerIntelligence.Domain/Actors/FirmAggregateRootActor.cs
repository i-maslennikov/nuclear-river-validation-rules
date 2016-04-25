using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Commands;
using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.CustomerIntelligence.Domain.Specifications;
using NuClear.Replication.Core.API.Actors;
using NuClear.Replication.Core.API.DataObjects;
using NuClear.Replication.Core.API.Equality;
using NuClear.River.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Actors
{
    public sealed class FirmAggregateRootActor : EntityActorBase<Firm>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<FirmActivity> _firmActivityBulkRepository;
        private readonly IBulkRepository<FirmBalance> _firmBalanceBulkRepository;
        private readonly IBulkRepository<FirmCategory1> _firmCategory1BulkRepository;
        private readonly IBulkRepository<FirmCategory2> _firmCategory2BulkRepository;
        private readonly IBulkRepository<FirmTerritory> _firmFirmTerritoryBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public FirmAggregateRootActor(
            IQuery query,
            IBulkRepository<Firm> firmBulkRepository,
            IBulkRepository<FirmActivity> firmActivityBulkRepository,
            IBulkRepository<FirmBalance> firmBalanceBulkRepository,
            IBulkRepository<FirmCategory1> firmCategory1BulkRepository,
            IBulkRepository<FirmCategory2> firmCategory2BulkRepository,
            IBulkRepository<FirmTerritory> firmFirmTerritoryBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, firmBulkRepository, equalityComparerFactory, new FirmAccessor(query))
        {
            _query = query;
            _firmActivityBulkRepository = firmActivityBulkRepository;
            _firmBalanceBulkRepository = firmBalanceBulkRepository;
            _firmCategory1BulkRepository = firmCategory1BulkRepository;
            _firmCategory2BulkRepository = firmCategory2BulkRepository;
            _firmFirmTerritoryBulkRepository = firmFirmTerritoryBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }


        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<FirmActivity>(_query, _firmActivityBulkRepository, _equalityComparerFactory, new FirmActivityAccessor(_query)),
                    new ValueObjectActor<FirmBalance>(_query, _firmBalanceBulkRepository, _equalityComparerFactory, new FirmBalanceAccessor(_query)),
                    new ValueObjectActor<FirmCategory1>(_query, _firmCategory1BulkRepository, _equalityComparerFactory, new FirmCategory1Accessor(_query)),
                    new ValueObjectActor<FirmCategory2>(_query, _firmCategory2BulkRepository, _equalityComparerFactory, new FirmCategory2Accessor(_query)),
                    new ValueObjectActor<FirmTerritory>(_query, _firmFirmTerritoryBulkRepository, _equalityComparerFactory, new FirmTerritoryAccessor(_query))
                };

        public sealed class FirmAccessor : IStorageBasedDataObjectAccessor<Firm>
        {
            private readonly IQuery _query;

            public FirmAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Firm> GetSource() => Specs.Map.Facts.ToCI.Firms.Map(_query);

            public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => new FindSpecification<Firm>(x => commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().Contains(x.Id));
        }

        public sealed class FirmActivityAccessor : IStorageBasedDataObjectAccessor<FirmActivity>
        {
            private readonly IQuery _query;

            public FirmActivityAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmActivity> GetSource() => Specs.Map.Facts.ToCI.FirmActivities.Map(_query);

            public FindSpecification<FirmActivity> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmActivities(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        public sealed class FirmBalanceAccessor : IStorageBasedDataObjectAccessor<FirmBalance>
        {
            private readonly IQuery _query;

            public FirmBalanceAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmBalance> GetSource() => Specs.Map.Facts.ToCI.FirmBalances.Map(_query);

            public FindSpecification<FirmBalance> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmBalances(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        public sealed class FirmCategory1Accessor : IStorageBasedDataObjectAccessor<FirmCategory1>
        {
            private readonly IQuery _query;

            public FirmCategory1Accessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmCategory1> GetSource() => Specs.Map.Facts.ToCI.FirmCategories1.Map(_query);

            public FindSpecification<FirmCategory1> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmCategories1(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        public sealed class FirmCategory2Accessor : IStorageBasedDataObjectAccessor<FirmCategory2>
        {
            private readonly IQuery _query;

            public FirmCategory2Accessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmCategory2> GetSource() => Specs.Map.Facts.ToCI.FirmCategories2.Map(_query);

            public FindSpecification<FirmCategory2> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmCategories2(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }

        public sealed class FirmTerritoryAccessor : IStorageBasedDataObjectAccessor<FirmTerritory>
        {
            private readonly IQuery _query;

            public FirmTerritoryAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<FirmTerritory> GetSource() => Specs.Map.Facts.ToCI.FirmTerritories.Map(_query);

            public FindSpecification<FirmTerritory> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
                => Specs.Find.CI.FirmTerritories(commands.Cast<IAggregateCommand>().Select(c => c.AggregateRootId).Distinct().ToArray());
        }
    }
}