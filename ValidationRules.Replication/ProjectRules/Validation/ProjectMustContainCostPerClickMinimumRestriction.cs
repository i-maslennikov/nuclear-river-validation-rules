using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для проектов, где есть продажи в рубрики без указанного ограничения стоимости клика, должна выводиться ошибка.
    /// "Для рубрики {0} в проекте {1} не указан минимальный CPC"
    /// 
    /// Source: IsCostPerClickRestrictionMissingOrderValidationRule
    /// </summary>
    public sealed class ProjectMustContainCostPerClickMinimumRestriction : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Error)
                                                                    .WhenMass(Result.Error)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public ProjectMustContainCostPerClickMinimumRestriction(IQuery query) : base(query, MessageTypeCode.ProjectMustContainCostPerClickMinimumRestriction)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from project in query.For<Project>()
                from order in query.For<Order>().Where(x => x.ProjectId == project.Id)
                from bid in query.For<Order.CostPerClickAdvertisement>().Where(x => x.OrderId == order.Id)
                let restrictionExist = query.For<Project.CostPerClickRestriction>().Any(x => x.ProjectId == order.ProjectId && x.CategoryId == bid.CategoryId)
                where !restrictionExist
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeCategory>(bid.CategoryId),
                                    new Reference<EntityTypeProject>(order.ProjectId),
                                    new Reference<EntityTypeOrder>(order.Id))
                                .ToXDocument(),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,

                        Result = RuleResult,
                    };

            return ruleResults;
        }
    }
}