using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.Actors;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.Actors
{
    public sealed class PriceAggregateRootActor : EntityActorBase<Price>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<AdvertisementAmountRestriction> _advertisementAmountRestrictionBulkRepository;
        private readonly IBulkRepository<PriceDeniedPosition> _priceDeniedPositionBulkRepository;
        private readonly IBulkRepository<PriceAssociatedPosition> _priceAssociatedPositionBulkRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public PriceAggregateRootActor(
            IQuery query,
            IBulkRepository<Price> bulkRepository,
            IBulkRepository<AdvertisementAmountRestriction> advertisementAmountRestrictionBulkRepository,
            IBulkRepository<PriceDeniedPosition> priceDeniedPositionBulkRepository,
            IBulkRepository<PriceAssociatedPosition> priceAssociatedPositionBulkRepository,
            IEqualityComparerFactory equalityComparerFactory)
            : base(query, bulkRepository, equalityComparerFactory, new PriceAccessor(query))
        {
            _query = query;
            _advertisementAmountRestrictionBulkRepository = advertisementAmountRestrictionBulkRepository;
            _priceDeniedPositionBulkRepository = priceDeniedPositionBulkRepository;
            _priceAssociatedPositionBulkRepository = priceAssociatedPositionBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
        }


        public IReadOnlyCollection<IEntityActor> GetEntityActors() => new IEntityActor[0];

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<AdvertisementAmountRestriction>(_query, _advertisementAmountRestrictionBulkRepository, _equalityComparerFactory, new AdvertisementAmountRestrictionAccessor(_query)),
                    new ValueObjectActor<PriceDeniedPosition>(_query, _priceDeniedPositionBulkRepository, _equalityComparerFactory, new PriceDeniedPositionAccessor(_query)),
                    new ValueObjectActor<PriceAssociatedPosition>(_query, _priceAssociatedPositionBulkRepository, _equalityComparerFactory, new PriceAssociatedPositionAccessor(_query)),
                };

        public sealed class PriceAccessor : IStorageBasedDataObjectAccessor<Price>
        {
            private readonly IQuery _query;

            public PriceAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<Price> GetSource() => Specs.Map.Facts.ToAggregates.Prices.Map(_query);

            public FindSpecification<Price> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Price>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class AdvertisementAmountRestrictionAccessor : IStorageBasedDataObjectAccessor<AdvertisementAmountRestriction>
        {
            private readonly IQuery _query;

            public AdvertisementAmountRestrictionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<AdvertisementAmountRestriction> GetSource()
                => from pricePosition in _query.For<Facts::PricePosition>()
                   join position in _query.For<Facts::Position>().Where(x => x.IsControlledByAmount) on pricePosition.PositionId equals position.Id
                   group new { pricePosition.PriceId, position.CategoryCode, position.Name, pricePosition.MinAdvertisementAmount, pricePosition.MaxAdvertisementAmount }
                       by new { pricePosition.PriceId, position.CategoryCode } into groups
                   select new AdvertisementAmountRestriction
                       {
                           PriceId = groups.Key.PriceId,
                           CategoryCode = groups.Key.CategoryCode,
                           CategoryName = (from pp in _query.For<Facts::PricePosition>().Where(x => x.PriceId == groups.Key.PriceId)
                                           join p in _query.For<Facts::Position>().Where(x => x.IsControlledByAmount && x.CategoryCode == groups.Key.CategoryCode).OrderBy(x => x.Id) on pp.PositionId
                                               equals p.Id
                                           select p.Name).First(), // Этот кусок кода достаточно точно отражает текущее поведение в erm, решение лучше - создать справочник и слушать поток flowNomenclatures.NomenclatureCategory
                       Max = groups.Min(x => x.MaxAdvertisementAmount) ?? int.MaxValue,
                           Min = groups.Max(x => x.MinAdvertisementAmount) ?? 0,
                           MissingMinimalRestriction = groups.Any(x => x.MinAdvertisementAmount == null)
                       };

            public FindSpecification<AdvertisementAmountRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<AdvertisementAmountRestriction>(x => aggregateIds.Contains(x.PriceId));
            }
        }

        public sealed class PriceDeniedPositionAccessor : IStorageBasedDataObjectAccessor<PriceDeniedPosition>
        {
            private readonly IQuery _query;

            public PriceDeniedPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<PriceDeniedPosition> GetSource() => Specs.Map.Facts.ToAggregates.PriceDeniedPositions.Map(_query);

            public FindSpecification<PriceDeniedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.Aggs.PriceDeniedPositions(aggregateIds);
            }
        }

        public sealed class PriceAssociatedPositionAccessor : IStorageBasedDataObjectAccessor<PriceAssociatedPosition>
        {
            private readonly IQuery _query;

            public PriceAssociatedPositionAccessor(IQuery query)
            {
                _query = query;
            }

            public IQueryable<PriceAssociatedPosition> GetSource() => Specs.Map.Facts.ToAggregates.PriceAssociatedPositions.Map(_query);

            public FindSpecification<PriceAssociatedPosition> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return Specs.Find.Aggs.PriceAssociatedPositions(aggregateIds);
            }
        }
    }
}