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
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.PriceRules.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class PriceAggregateRootActor : EntityActorBase<Price>, IAggregateRootActor
    {
        private readonly IQuery _query;
        private readonly IBulkRepository<AdvertisementAmountRestriction> _advertisementAmountRestrictionBulkRepository;
        private readonly IBulkRepository<AssociatedPositionGroupOvercount> _associatedPositionGroupOvercountRepository;
        private readonly IEqualityComparerFactory _equalityComparerFactory;

        public PriceAggregateRootActor(
            IQuery query,
            IBulkRepository<Price> bulkRepository,
            IBulkRepository<AdvertisementAmountRestriction> advertisementAmountRestrictionBulkRepository,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<AssociatedPositionGroupOvercount> associatedPositionGroupOvercountRepository)
            : base(query, bulkRepository, equalityComparerFactory, new PriceAccessor(query))
        {
            _query = query;
            _advertisementAmountRestrictionBulkRepository = advertisementAmountRestrictionBulkRepository;
            _equalityComparerFactory = equalityComparerFactory;
            _associatedPositionGroupOvercountRepository = associatedPositionGroupOvercountRepository;
        }

        public IReadOnlyCollection<IEntityActor> GetEntityActors() => Array.Empty<IEntityActor>();

        public override IReadOnlyCollection<IActor> GetValueObjectActors()
            => new IActor[]
                {
                    new ValueObjectActor<AdvertisementAmountRestriction>(_query, _advertisementAmountRestrictionBulkRepository, _equalityComparerFactory, new AdvertisementAmountRestrictionAccessor(_query)),
                    new ValueObjectActor<AssociatedPositionGroupOvercount>(_query, _associatedPositionGroupOvercountRepository, _equalityComparerFactory, new AssociatedPositionGroupOvercountAccessor(_query)),
                };

        public sealed class PriceAccessor : AggregateDataChangesHandler<Price>, IStorageBasedDataObjectAccessor<Price>
        {
            private readonly IQuery _query;

            public PriceAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AssociatedPositionsGroupCount);

                _query = query;
            }

            public IQueryable<Price> GetSource()
                => _query.For<Facts::Price>().Select(price => new Price { Id = price.Id, BeginDate = price.BeginDate });

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

        public sealed class AdvertisementAmountRestrictionAccessor : AggregateDataChangesHandler<AdvertisementAmountRestriction>, IStorageBasedDataObjectAccessor<AdvertisementAmountRestriction>
        {
            private readonly IQuery _query;

            public AdvertisementAmountRestrictionAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.MaximumAdvertisementAmount);
                Invalidate(MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified);
                Invalidate(MessageTypeCode.MinimumAdvertisementAmount);

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

        public sealed class AssociatedPositionGroupOvercountAccessor : AggregateDataChangesHandler<AssociatedPositionGroupOvercount>, IStorageBasedDataObjectAccessor<AssociatedPositionGroupOvercount>
        {
            // Предполагается, что когда начнём создавать события на втором этапе - события этого класса будут приводить к вызову одной соответствующей проверки
            private readonly IQuery _query;

            public AssociatedPositionGroupOvercountAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.AssociatedPositionsGroupCount);

                _query = query;
            }

            public IQueryable<AssociatedPositionGroupOvercount> GetSource()
                => from pricePosition in _query.For<Facts::PricePosition>()
                   let count = _query.For<Facts::AssociatedPositionsGroup>().Count(x => x.PricePositionId == pricePosition.Id)
                   let name = _query.For<Facts::Position>().Single(x => x.Id == pricePosition.PositionId).Name
                   where count > 1
                   select new AssociatedPositionGroupOvercount
                   {
                       PriceId = pricePosition.PriceId,
                       PricePositionId = pricePosition.Id,
                       PricePositionName = name,
                       Count = count,
                   };

            public FindSpecification<AssociatedPositionGroupOvercount> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<AssociatedPositionGroupOvercount>(x => aggregateIds.Contains(x.PriceId));
            }
        }
    }
}