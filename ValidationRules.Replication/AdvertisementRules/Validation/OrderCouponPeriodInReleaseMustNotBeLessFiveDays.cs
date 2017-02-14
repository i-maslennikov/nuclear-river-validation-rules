using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для ЭРМ с указанными датами размещения, срок размещения не должн быть менее пяти дней в релизе
    /// 
    /// "Период размещения рекламного материала {0}, выбранного в позиции {1} должен захватывать 5 дней от текущего месяца размещения"
    /// 
    /// Source: CouponPeriodOrderValidationRule/AdvertisementPeriodEndsBeforeReleasePeriodBegins
    /// </summary>
    public sealed class OrderCouponPeriodInReleaseMustNotBeLessFiveDays : ValidationResultAccessorBase
    {
        private const int MaxOffsetInDays = 5;

        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderCouponPeriodInReleaseMustNotBeLessFiveDays(IQuery query) : base(query, MessageTypeCode.OrderCouponPeriodInReleaseMustNotBeLessFiveDays)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from opa in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id)
                from advertisement in query.For<Advertisement>().Where(x => x.Id == opa.AdvertisementId)
                from elementOffset in query.For<Advertisement.Coupon>().Where(x => x.AdvertisementId == advertisement.Id)
                where elementOffset.DaysTotal < MaxOffsetInDays ||
                      elementOffset.DaysFromMonthBeginToCouponEnd < MaxOffsetInDays ||
                      elementOffset.DaysFromCouponBeginToMonthEnd < MaxOffsetInDays
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(opa.OrderPositionId),
                                        new Reference<EntityTypePosition>(opa.PositionId)),
                                    new Reference<EntityTypeAdvertisement>(advertisement.Id))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDatePlan,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}
