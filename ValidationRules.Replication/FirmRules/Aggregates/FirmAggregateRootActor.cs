using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Replication.Core;
using NuClear.Replication.Core.DataObjects;
using NuClear.Replication.Core.Equality;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;
using NuClear.ValidationRules.Replication.Commands;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Facts = NuClear.ValidationRules.Storage.Model.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Aggregates
{
    public sealed class FirmAggregateRootActor : AggregateRootActor<Firm>
    {
        public FirmAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Firm> bulkRepository,
            IBulkRepository<Firm.AdvantageousPurchasePositionDistributionPeriod> advantageousPurchasePositionDistributionPeriodRepository,
            IBulkRepository<Firm.CategoryPurchase> categoryPurchaseRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new FirmAccessor(query), bulkRepository,
                HasValueObject(new AdvantageousPurchasePositionDistributionPeriodAccessor(query), advantageousPurchasePositionDistributionPeriodRepository),
                HasValueObject(new CategoryPurchaseAccessor(query), categoryPurchaseRepository));
        }

        public sealed class FirmAccessor : DataChangesHandler<Firm>, IStorageBasedDataObjectAccessor<Firm>
        {
            private readonly IQuery _query;

            public FirmAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit,
                        MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                        MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                        MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder,
                    };

            public IQueryable<Firm> GetSource()
                => from firm in _query.For<Facts::Firm>()
                   from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == firm.OrganizationUnitId)
                   select new Firm
                       {
                           Id = firm.Id,
                           ProjectId = project.Id,
                       };

            public FindSpecification<Firm> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Firm>(x => aggregateIds.Contains(x.Id));
            }
        }

        public sealed class AdvantageousPurchasePositionDistributionPeriodAccessor : DataChangesHandler<Firm.AdvantageousPurchasePositionDistributionPeriod>, IStorageBasedDataObjectAccessor<Firm.AdvantageousPurchasePositionDistributionPeriod>
        {
            private readonly IQuery _query;

            public AdvantageousPurchasePositionDistributionPeriodAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases,
                        MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder,
                    };

            public IQueryable<Firm.AdvantageousPurchasePositionDistributionPeriod> GetSource()
            {
                var firmsWithCategory =
                    from firmAddressCategory in _query.For<Facts::FirmAddressCategory>().Where(x => x.CategoryId == Facts::Category.AdvantageousPurchaseWith2Gis)
                    from firmAddress in _query.For<Facts::FirmAddress>().Where(x => x.IsActive && !x.IsDeleted && !x.IsClosedForAscertainment).Where(x => x.Id == firmAddressCategory.FirmAddressId)
                    from firm in _query.For<Facts::Firm>().Where(x => x.IsActive && !x.IsDeleted && !x.IsClosedForAscertainment).Where(x => x.Id == firmAddress.FirmId)
                    select firm;

                var periodsForAllFirms =
                    from firm in _query.For<Facts::Firm>()
                    select new { FirmId = firm.Id, Begin = DateTime.MinValue, End = DateTime.MaxValue, Has = false, Scope = Scope.ApprovedScope };

                var specialPositions = _query.For<Facts::Position>().Where(x => !x.IsDeleted).Select(x => new
                {
                    x.Id,
                    IsAdvantageousPurchaseOnPc = x.CategoryCode == Facts::Position.CategoryCodeAdvantageousPurchaseWith2Gis && x.Platform == Facts::Position.PlatformDesktop,
                    IsSelfAdvertisementOnPc = x.CategoryCode == Facts::Position.CategoryCodeSelfAdvertisementOnlyOnPc,
                });

                var periodsForAllOrders =
                    from order in _query.For<Facts::Order>()
                    let has = (from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                               from orderPositionAdvertisement in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                               from specialPosition in specialPositions.Where(x => x.Id == orderPositionAdvertisement.PositionId)
                               select specialPosition).Any(x => x.IsAdvantageousPurchaseOnPc || x.IsSelfAdvertisementOnPc)
                    let scope = Scope.Compute(order.WorkflowStep, order.Id)
                    select new { order.FirmId, Begin = order.BeginDistribution, End = order.EndDistributionFact, Has = has, Scope = scope };

                var periodsForFirmsWithCategory =
                    from firm in firmsWithCategory.Distinct()
                    from period in periodsForAllOrders.Union(periodsForAllFirms).Where(x => x.FirmId == firm.Id)
                    select new Firm.AdvantageousPurchasePositionDistributionPeriod
                        {
                            FirmId = firm.Id,
                            Scope = period.Scope,
                            Begin = period.Begin,
                            End = period.End,
                            HasPosition = period.Has,
                        };

                return periodsForFirmsWithCategory;
            }

            public FindSpecification<Firm.AdvantageousPurchasePositionDistributionPeriod> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.Cast<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Firm.AdvantageousPurchasePositionDistributionPeriod>(x => aggregateIds.Contains(x.FirmId));
            }
        }

        public sealed class CategoryPurchaseAccessor : DataChangesHandler<Firm.CategoryPurchase>, IStorageBasedDataObjectAccessor<Firm.CategoryPurchase>
        {
            private readonly IQuery _query;

            public CategoryPurchaseAccessor(IQuery query) : base(CreateInvalidator())
            {
                _query = query;
            }

            private static IRuleInvalidator CreateInvalidator()
                => new RuleInvalidator
                    {
                        MessageTypeCode.FirmShouldHaveLimitedCategoryCount,
                    };

            public IQueryable<Firm.CategoryPurchase> GetSource()
            {
                var dates =
                    _query.For<Facts::Order>()
                          .Select(x => new { Date = x.BeginDistribution, x.FirmId })
                          .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionFact, x.FirmId }))
                          .Union(_query.For<Facts::Order>().Select(x => new { Date = x.EndDistributionPlan, x.FirmId }));

                var cats =
                    _query.For<Facts::OrderItem>()
                          .Where(x => x.CategoryId.HasValue)
                          .Select(x => new { x.OrderId, CategoryId = x.CategoryId.Value })
                          .Distinct();

                var result =
                    from order in _query.For<Facts::Order>()
                    from cat in cats.Where(x => x.OrderId == order.Id)
                    from date in dates.Where(x => x.FirmId == order.FirmId && order.BeginDistribution <= x.Date && x.Date < order.EndDistributionPlan)
                    from nextDate in dates.Where(x => x.FirmId == order.FirmId && x.Date > date.Date).OrderBy(x => x.Date).Take(1)
                    select new Firm.CategoryPurchase
                        {
                            FirmId = order.FirmId,
                            CategoryId = cat.CategoryId,
                            Begin = date.Date,
                            End = nextDate.Date,
                            Scope = order.EndDistributionFact > date.Date ? Scope.Compute(order.WorkflowStep, order.Id) : order.Id,
                        };

                result = result.Distinct();

                return result;
            }

            public FindSpecification<Firm.CategoryPurchase> GetFindSpecification(IReadOnlyCollection<ICommand> commands)
            {
                var aggregateIds = commands.OfType<ReplaceValueObjectCommand>().Select(c => c.AggregateRootId).Distinct().ToArray();
                return new FindSpecification<Firm.CategoryPurchase>(x => aggregateIds.Contains(x.FirmId));
            }
        }
    }
}

