using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, к которым привязана неактуальная рубрика должно выводиться ошибка.
    /// "В позиции {0} найдена неактивная рубрика {1}"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// </summary>
    public sealed class LinkedCategoryShouldBeActive : ValidationResultAccessorBase
    {
        public LinkedCategoryShouldBeActive(IQuery query) : base(query, MessageTypeCode.LinkedCategoryShouldBeActive)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from category in query.For<Order.InvalidCategory>().Where(x => x.State == InvalidCategoryState.Inactive).Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeCategory>(category.CategoryId),
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(category.OrderPositionId),
                                        new Reference<EntityTypePosition>(category.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
