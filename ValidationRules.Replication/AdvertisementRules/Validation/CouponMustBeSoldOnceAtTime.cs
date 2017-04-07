using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Replication.Specifications;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для фирм (но выводим в заказы), размещающих один РМ в более чем одной позиции "выгодные покупки", должна выводиться ошибка
    /// "Купон на скидку {0} прикреплён к нескольким позициям: {1}"
    /// 
    /// Source: CouponIsUniqueForFirmOrderValidationRule/CouponIsBoundToMultiplePositionTemplate
    /// </summary>
    public sealed class CouponMustBeSoldOnceAtTime : ValidationResultAccessorBase
    {
        public CouponMustBeSoldOnceAtTime(IQuery query) : base(query, MessageTypeCode.CouponMustBeSoldOnceAtTime)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var result =
                from period in query.For<Order.CouponDistributionPeriod>()
                    .Where(x => query.For<Order.CouponDistributionPeriod>().Count(y => x.AdvertisementId == y.AdvertisementId && x.Begin < y.End && y.Begin < x.End && Scope.CanSee(x.Scope, y.Scope)) > 1)
                from other in query.For<Order.CouponDistributionPeriod>().Where(x => period.AdvertisementId == x.AdvertisementId && period.Begin < x.End && x.Begin < period.End && Scope.CanSee(period.Scope, x.Scope))
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeAdvertisement>(period.AdvertisementId),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(other.OrderPositionId),
                                        new Reference<EntityTypePosition>(other.PositionId)))
                                .ToXDocument(),

                        PeriodStart = period.Begin,
                        PeriodEnd = period.End,
                        OrderId = period.OrderId,
                    };

            return result;
        }
    }
}
