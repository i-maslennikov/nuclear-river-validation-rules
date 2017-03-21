using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых дата начала размещения не является первым днём месяца, должна выводиться ошибка.
    /// "Указана некорректная дата начала размещения"
    /// 
    /// Source: DistributionDatesOrderValidationRule
    /// </summary>
    public sealed class OrderBeginDistrubutionShouldBeFirstDayOfMonth : ValidationResultAccessorBase
    {
        public OrderBeginDistrubutionShouldBeFirstDayOfMonth(IQuery query) : base(query, MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from date in query.For<Order.InvalidBeginDistributionDate>().Where(x => x.OrderId == order.Id)
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
