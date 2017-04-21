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
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.PriceRules.Aggregates
{
    public sealed class PriceAggregateRootActor : AggregateRootActor<Price>
    {
        public PriceAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Price> bulkRepository,
            IBulkRepository<Price.PricePeriod> pricePeriodBulkRepository,
            IBulkRepository<Price.AdvertisementAmountRestriction> advertisementAmountRestrictionBulkRepository,
            IBulkRepository<Price.AssociatedPositionGroupOvercount> associatedPositionGroupOvercountRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new PriceAccessor(query), bulkRepository,
                HasValueObject(new PricePeriodAccessor(query), pricePeriodBulkRepository),
                HasValueObject(new AdvertisementAmountRestrictionAccessor(query), advertisementAmountRestrictionBulkRepository),
                HasValueObject(new AssociatedPositionGroupOvercountAccessor(query), associatedPositionGroupOvercountRepository));
        }

        public sealed class PriceAccessor : DataChangesHandler<Price>, IStorageBasedDataObjectAccessor<Price>
        {
            private readonly IQuery _query;

            public PriceAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AssociatedPositionsGroupCount,
                    };

            public IQueryable<Price> GetSource()
                => _query.For<Facts::Price>().Select(price => new Price { Id = price.Id });

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

        public sealed class AdvertisementAmountRestrictionAccessor : DataChangesHandler<Price.AdvertisementAmountRestriction>, IStorageBasedDataObjectAccessor<Price.AdvertisementAmountRestriction>
        {
            private readonly IQuery _query;

            public AdvertisementAmountRestrictionAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.MaximumAdvertisementAmount,
                        MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified,
                        MessageTypeCode.MinimumAdvertisementAmount
                    };

            public IQueryable<Price.AdvertisementAmountRestriction> GetSource()
                => from pricePosition in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted)
                   join position in _query.For<Facts::Position>().Where(x => !x.IsDeleted && x.IsControlledByAmount) on pricePosition.PositionId equals position.Id
                   group new { pricePosition.MinAdvertisementAmount, pricePosition.MaxAdvertisementAmount } by new { pricePosition.PriceId, position.CategoryCode } into groups
                   from nomencalure in _query.For<Facts.NomenclatureCategory>().Where(x => x.Id == groups.Key.CategoryCode && x.PriceId == groups.Key.PriceId).DefaultIfEmpty()
                   select new Price.AdvertisementAmountRestriction
                       {
                           PriceId = groups.Key.PriceId,
                           CategoryCode = groups.Key.CategoryCode,
                           CategoryName = nomencalure == null ? "empty" : nomencalure.Name,
                           Max = groups.Min(x => x.MaxAdvertisementAmount) ?? int.MaxValue,
                           Min = groups.Max(x => x.MinAdvertisementAmount) ?? 0,
                           MissingMinimalRestriction = groups.Max(x => x.MinAdvertisementAmount) == null
                       };

            public FindSpecification<Price.AdvertisementAmountRestriction> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Price.AdvertisementAmountRestriction>(x => aggregateIds.Contains(x.PriceId));
            }
        }

        public sealed class AssociatedPositionGroupOvercountAccessor : DataChangesHandler<Price.AssociatedPositionGroupOvercount>, IStorageBasedDataObjectAccessor<Price.AssociatedPositionGroupOvercount>
        {
            // Предполагается, что когда начнём создавать события на втором этапе - события этого класса будут приводить к вызову одной соответствующей проверки
            private readonly IQuery _query;

            public AssociatedPositionGroupOvercountAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AssociatedPositionsGroupCount
                    };

            public IQueryable<Price.AssociatedPositionGroupOvercount> GetSource()
                => from pricePosition in _query.For<Facts::PricePosition>().Where(x => x.IsActiveNotDeleted)
                   let count = _query.For<Facts::AssociatedPositionsGroup>().Count(x => x.PricePositionId == pricePosition.Id)
                   where count > 1
                   select new Price.AssociatedPositionGroupOvercount
                   {
                       PriceId = pricePosition.PriceId,
                       PricePositionId = pricePosition.Id,
                       PositionId = pricePosition.PositionId,
                       Count = count,
                   };

            public FindSpecification<Price.AssociatedPositionGroupOvercount> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Price.AssociatedPositionGroupOvercount>(x => aggregateIds.Contains(x.PriceId));
            }
        }

        public sealed class PricePeriodAccessor : DataChangesHandler<Price.PricePeriod>, IStorageBasedDataObjectAccessor<Price.PricePeriod>
        {
            private readonly IQuery _query;

            public PricePeriodAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.AssociatedPositionsGroupCount
                    };

            public IQueryable<Price.PricePeriod> GetSource()
            {
                var result =
                    from price in _query.For<Facts::Price>()
                    let nextPrice = _query.For<Facts::Price>().Where(x => x.OrganizationUnitId == price.OrganizationUnitId && x.BeginDate > price.BeginDate).Min(x => (DateTime?)x.BeginDate)
                    from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == price.OrganizationUnitId)
                    select new Price.PricePeriod
                        {
                            PriceId = price.Id,
                            Begin = price.BeginDate,
                            End = nextPrice ?? DateTime.MaxValue,
                            ProjectId = project.Id,
                        };

                return result;
            }

            public FindSpecification<Price.PricePeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Price.PricePeriod>(x => aggregateIds.Contains(x.PriceId));
            }
        }

    }
}