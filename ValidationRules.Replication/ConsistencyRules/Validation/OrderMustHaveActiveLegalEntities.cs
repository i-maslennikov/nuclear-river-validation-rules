using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.ConsistencyRules.Validation
{
    /// <summary>
    /// Для заказов, которые связаны с неактиыми или удалёнными объектами BranchOfficeOrganizationUnit, BranchOffice, LegalPerson, LegalPersonProfile, должна выводиться ошибка
    /// "Заказ ссылается на неактивные объекты: {0}"
    /// 
    /// Source: OrderHasActiveLegalDetailsOrderValidationRule
    /// </summary>
    public sealed class OrderMustHaveActiveLegalEntities : ValidationResultAccessorBase
    {
        public OrderMustHaveActiveLegalEntities(IQuery query) : base(query, MessageTypeCode.OrderMustHaveActiveLegalEntities)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from inactive in query.For<Order.InactiveReference>().Where(x => x.OrderId == order.Id)
                where inactive.BranchOffice || inactive.BranchOfficeOrganizationUnit || inactive.LegalPerson || inactive.LegalPersonProfile
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object>
                                        {
                                                { "branchOfficeOrganizationUnit", inactive.BranchOfficeOrganizationUnit },
                                                { "branchOffice", inactive.BranchOffice },
                                                { "legalPerson", inactive.LegalPerson },
                                                { "legalPersonProfile", inactive.LegalPersonProfile },
                                        },
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
