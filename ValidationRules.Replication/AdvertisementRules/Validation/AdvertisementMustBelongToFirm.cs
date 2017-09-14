using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, в РМ которых указана фирма, не являющейся фирмой заказа, должна выводиться ошибка
    /// -В позиции {0} выбран рекламный материал {1}, не принадлежащий фирме {2}
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/AdvertisementSpecifiedForPositionDoesNotBelongToFirm
    /// </summary>
    public sealed class AdvertisementMustBelongToFirm : ValidationResultAccessorBase
    {
        public AdvertisementMustBelongToFirm(IQuery query) : base(query, MessageTypeCode.AdvertisementMustBelongToFirm)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from fail in query.For<Order.AdvertisementNotBelongToFirm>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(fail.OrderId),
                                    new Reference<EntityTypeOrderPosition>(fail.OrderPositionId,
                                        new Reference<EntityTypeOrder>(fail.OrderId),
                                        new Reference<EntityTypePosition>(fail.PositionId)),
                                    new Reference<EntityTypeAdvertisement>(fail.AdvertisementId),
                                    new Reference<EntityTypeFirm>(fail.ExpectedFirmId))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDatePlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}