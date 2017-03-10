using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
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
                        MessageParams =
                            new MessageParams(
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