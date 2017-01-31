using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для заказа, который оформлен на период по которому уже создан ReleaseInfo, должна выводиться ошибка.
    /// "Заказ оформлен на период, по которому уже сформирована сборка. Необходимо указать другие даты размещения заказа"
    /// 
    /// Source: ReleaseNotExistsOrderValidationRule
    /// 
    /// Примечание: не должна проводиться по "одобренным" заказам
    /// </summary>
    public sealed class OrderMustNotIncludeReleasedPeriod : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.None)
                                                                    .WhenMassRelease(Result.None);

        public OrderMustNotIncludeReleasedPeriod(IQuery query) : base(query, MessageTypeCode.OrderMustNotIncludeReleasedPeriod)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>().Where(x => x.IsDraft)
                from project in query.For<Project.NextRelease>().Where(x => x.ProjectId == order.ProjectId)
                where project.Date > order.Begin
                select new Version.ValidationResult
                    {
                        MessageParams = new XDocument(
                            new XElement("root",
                                new XElement("order",
                                    new XAttribute("id", order.Id)))),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}