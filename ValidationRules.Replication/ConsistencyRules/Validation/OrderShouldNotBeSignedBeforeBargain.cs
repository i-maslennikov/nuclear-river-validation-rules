using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

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
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

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

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}
