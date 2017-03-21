using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов с продажами в рубрику адреса, к которым привязана рубрика не принадлежащая адресу, должно выводиться предупреждение в единичной, ошибка в массовой.
    /// "В позиции {0} найдена рубрика {1}, не принадлежащая адресу {2}"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// </summary>
    public sealed class LinkedCategoryFirmAddressShouldBeValid : ValidationResultAccessorBase
    {
        public LinkedCategoryFirmAddressShouldBeValid(IQuery query) : base(query, MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from categoryFirmAddress in query.For<Order.CategoryNotBelongsToAddress>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeFirmAddress>(categoryFirmAddress.FirmAddressId),
                                    new Reference<EntityTypeCategory>(categoryFirmAddress.CategoryId),
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(categoryFirmAddress.OrderPositionId),
                                        new Reference<EntityTypePosition>(categoryFirmAddress.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
