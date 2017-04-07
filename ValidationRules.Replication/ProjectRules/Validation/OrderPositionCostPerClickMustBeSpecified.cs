using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для заказов, с продажами в рубрики для которых не указана стоимость клика, должна выводиться ошибка
    /// "Для позиции {0} в рубрику {1} отсутствует CPC"
    /// 
    /// Source: IsCostPerClickMissingOrderValidationRule
    /// </summary>
    public sealed class OrderPositionCostPerClickMustBeSpecified : ValidationResultAccessorBase
    {
        public const int CostPerClickSalesModel = 12; // erm: MultiPlannedProvision

        public OrderPositionCostPerClickMustBeSpecified(IQuery query) : base(query, MessageTypeCode.OrderPositionCostPerClickMustBeSpecified)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from adv in query.For<Order.CategoryAdvertisement>().Where(x => x.SalesModel == CostPerClickSalesModel).Where(x => x.OrderId == order.Id)
                where !query.For<Order.CostPerClickAdvertisement>().Any(x => x.OrderPositionId == adv.OrderPositionId && x.CategoryId == adv.CategoryId)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeCategory>(adv.CategoryId),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(adv.OrderPositionId),
                                        new Reference<EntityTypePosition>(adv.PositionId)),
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}