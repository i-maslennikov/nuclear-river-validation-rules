using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых дата подписания ранее даты подписания договора, должна выводиться ошибка.
    /// "Договор не может иметь дату подписания позднее даты подписания заказа"
    /// 
    /// Source: BargainAndOrderSignDatesValidationRule
    /// </summary>
    public sealed class OrderShouldNotBeSignedBeforeBargain : ValidationResultAccessorBase
    {
        public OrderShouldNotBeSignedBeforeBargain(IQuery query) : base(query, MessageTypeCode.OrderShouldNotBeSignedBeforeBargain)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from date in query.For<Order.BargainSignedLaterThanOrder>().Where(x => x.OrderId == order.Id)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistribution,
                        PeriodEnd = order.EndDistributionPlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
