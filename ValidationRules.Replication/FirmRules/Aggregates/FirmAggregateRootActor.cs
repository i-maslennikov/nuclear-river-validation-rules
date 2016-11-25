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

using Facts = NuClear.ValidationRules.Storage.Model.FirmRules.Facts;

namespace NuClear.ValidationRules.Replication.FirmRules.Aggregates
{
    public sealed class FirmAggregateRootActor : AggregateRootActor<Firm>
    {
        public FirmAggregateRootActor(
            IQuery query,
            IEqualityComparerFactory equalityComparerFactory,
            IBulkRepository<Firm> bulkRepository,
            IBulkRepository<Firm.AdvantageousPurchasePositionDistributionPeriod> advantageousPurchasePositionDistributionPeriodRepository)
            : base(query, equalityComparerFactory)
        {
            HasRootEntity(new FirmAccessor(query), bulkRepository,
                HasValueObject(new AdvantageousPurchasePositionDistributionPeriodAccessor(query), advantageousPurchasePositionDistributionPeriodRepository));
        }

        public sealed class FirmAccessor : DataChangesHandler<Firm>, IStorageBasedDataObjectAccessor<Firm>
        {
            private readonly IQuery _query;

            public FirmAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.FirmAndOrderShouldBelongTheSameOrganizationUnit);
                Invalidate(MessageTypeCode.FirmShouldHaveLimitedCategoryCount);
                Invalidate(MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases);
                Invalidate(MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder);

                _query = query;
            }

            public IQueryable<Firm> GetSource()
                => from firm in _query.For<Facts::Firm>()
                   from project in _query.For<Facts::Project>().Where(x => x.OrganizationUnitId == firm.OrganizationUnitId)
                   select new Firm
                       {
                           Id = firm.Id,
                           ProjectId = project.Id,
                           Name = firm.Name,
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
            private const long SpecialCategoryId = 18599; // Выгодные покупки с 2ГИС.

            private readonly IQuery _query;

            public AdvantageousPurchasePositionDistributionPeriodAccessor(IQuery query)
            {
                Invalidate(MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases);
                Invalidate(MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder);

                _query = query;
            }

            public IQueryable<Firm.AdvantageousPurchasePositionDistributionPeriod> GetSource()
            {
                var firmsWithCategory =
                    from firmAddressCategory in _query.For<Facts::FirmAddressCategory>().Where(x => x.CategoryId == SpecialCategoryId)
                    from firmAddress in _query.For<Facts::FirmAddress>().Where(x => x.Id == firmAddressCategory.FirmAddressId)
                    from firm in _query.For<Facts::Firm>().Where(x => x.Id == firmAddress.FirmId)
                    select firm;

                var periodsForAllFirms =
                    from firm in _query.For<Facts::Firm>()
                    select new { FirmId = firm.Id, Begin = DateTime.MinValue, End = DateTime.MaxValue, Has = false, Scope = Scope.ApprovedScope };

                var periodsForAllOrders =
                    from order in _query.For<Facts::Order>()
                    let has = (from orderPosition in _query.For<Facts::OrderPosition>().Where(x => x.OrderId == order.Id)
                               from orderPositionAdvertisement in _query.For<Facts::OrderPositionAdvertisement>().Where(x => x.OrderPositionId == orderPosition.Id)
                               from position in _query.For<Facts::SpecialPosition>().Where(x => x.Id == orderPositionAdvertisement.PositionId)
                               select position).Any(x => x.IsAdvantageousPurchaseOnPc || x.IsSelfAdvertisementOnPc)
                    let scope = Scope.Compute(order.WorkflowStep, order.Id)
                    select new { order.FirmId, Begin = order.BeginDistribution, End = order.EndDistributionFact, Has = has, Scope = scope };

                var periodsForFirmsWithCategory =
                    from firm in firmsWithCategory
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
                var aggregateIds = commands.OfType<CreateDataObjectCommand>().Select(c => c.DataObjectId)
                                           .Concat(commands.OfType<SyncDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Concat(commands.OfType<DeleteDataObjectCommand>().Select(c => c.DataObjectId))
                                           .Distinct()
                                           .ToArray();
                return new FindSpecification<Firm.AdvantageousPurchasePositionDistributionPeriod>(x => aggregateIds.Contains(x.FirmId));
            }
        }
    }
}

