using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AccountRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AccountRules.Validation
{
    /// <summary>
    /// Для заказов, у которых нет лицевого счёта, должна выводиться ошибка.
    /// "Заказ {0} не имеет привязки к лицевому счёту"
    /// 
    /// Source: AccountExistsOrderValidationRule
    /// </summary>
    public sealed class AccountShouldExist : ValidationResultAccessorBase
    {
        // В erm эта проверка не вызывается при ручной проверке, только при сборке (в том числе бете)
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.None)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);


        public AccountShouldExist(IQuery query) : base(query, MessageTypeCode.AccountShouldExist)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                where !order.AccountId.HasValue
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("order",
                                    new XAttribute("id", order.Id),
                                    new XAttribute("number", order.Number)))),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDate,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}
