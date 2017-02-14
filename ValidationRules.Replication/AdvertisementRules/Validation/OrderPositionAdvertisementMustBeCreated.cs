using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых есть сложная позиция с обязательным составом и не указан объект привязки для любой подпозиции, должна выводиться ошибка
    /// "В позиции {0} необходимо указать хотя бы один объект привязки для подпозиции '{1}'"
    /// 
    /// Source: AdvertisementsWithoutWhiteListOrderValidationRule/OrderCheckCompositePositionMustHaveLinkingObject
    ///
    /// * Ошибки по проверкам OrderPositionAdvertisementMustBeCreated и OrderPositionAdvertisementMustHaveAdvertisement
    /// визуально для пользователя не различаются. Можно объединить и оставить универсальное сообщение.
    /// </summary>
    public sealed class OrderPositionAdvertisementMustBeCreated : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public OrderPositionAdvertisementMustBeCreated(IQuery query) : base(query, MessageTypeCode.OrderPositionAdvertisementMustBeCreated)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                join fail in query.For<Order.MissingOrderPositionAdvertisement>() on order.Id equals fail.OrderId
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrderPosition>(fail.OrderPositionId,
                                        new Reference<EntityTypeOrder>(order.Id),
                                        new Reference<EntityTypePosition>(fail.CompositePositionId)),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(fail.OrderPositionId),
                                        new Reference<EntityTypePosition>(fail.PositionId)))
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
