using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AccountRules.Validation
{
    /// <summary>
    /// Для заказов, у которых есть блокировки, должна выводиться ошибка.
    /// "Заказ {0} имеет созданную блокировку на указанный период"
    /// 
    /// Source: LockNoExistsOrderValidationRule/OrdersCheckOrderHasLock
    /// </summary>
    public sealed class LockShouldNotExist : ValidationResultAccessorBase
    {
        // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public LockShouldNotExist(IQuery query) : base(query, MessageTypeCode.LockShouldNotExist)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                join @lock in query.For<Lock>() on order.Id equals @lock.OrderId
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("number", order.Number)))),

                        PeriodStart = @lock.Start,
                        PeriodEnd = @lock.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}
