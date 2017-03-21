using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, к которым привязана рубрика, не принадлежащая фирме, если объект привязки является "рубрика множественная со звёздочкой", должно выводиться информационное сообщение.
    /// "В позиции {0} найдена рубрика {1}, не принадлежащая фирме заказа"
    /// 
    /// Source: LinkingObjectsOrderValidationRule
    /// </summary>
    public sealed class LinkedCategoryAsterixMayBelongToFirm : ValidationResultAccessorBase
    {
        public LinkedCategoryAsterixMayBelongToFirm(IQuery query) : base(query, MessageTypeCode.LinkedCategoryAsterixMayBelongToFirm)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from category in query.For<Order.InvalidCategory>().Where(x => x.OrderId == order.Id)
                where category.State == InvalidCategoryState.NotBelongToFirm && category.MayNotBelongToFirm
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
