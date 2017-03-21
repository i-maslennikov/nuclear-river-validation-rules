using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, у которых есть договор и отсутствет его скан, должно выводиться предупреждение.
    /// "Отсутствует сканированная копия договора"
    /// 
    /// Source: OrdersAndBargainsScansExistOrderValidationRule
    /// </summary>
    public sealed class BargainScanShouldPresent : ValidationResultAccessorBase
    {
        public BargainScanShouldPresent(IQuery query) : base(query, MessageTypeCode.BargainScanShouldPresent)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from date in query.For<Order.MissingBargainScan>().Where(x => x.OrderId == order.Id)
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
