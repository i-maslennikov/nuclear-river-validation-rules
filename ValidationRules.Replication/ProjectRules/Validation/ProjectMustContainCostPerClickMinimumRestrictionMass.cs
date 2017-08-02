using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.ProjectRules.Aggregates;

namespace NuClear.ValidationRules.Replication.ProjectRules.Validation
{
    /// <summary>
    /// Для проектов, где есть продажи в рубрики без указанного ограничения стоимости клика*, должна выводиться ошибка.
    /// "Для рубрики {0} в проекте {1} не указан минимальный CPC"
    /// 
    /// * учитываются ограничения только самой свежей версии для города.
    /// Source: IsCostPerClickRestrictionMissingOrderValidationRule
    /// </summary>
    public sealed class ProjectMustContainCostPerClickMinimumRestrictionMass : ValidationResultAccessorBase
    {
        public ProjectMustContainCostPerClickMinimumRestrictionMass(IQuery query) : base(query, MessageTypeCode.ProjectMustContainCostPerClickMinimumRestrictionMass)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            // Даты, к наступлению которых требуется наличие действующих ограничений
            var requiredRestrictions =
                from order in query.For<Order>()
                from bid in query.For<Order.CostPerClickAdvertisement>().Where(x => x.OrderId == order.Id)
                let nextRelease = query.For<Project.NextRelease>().FirstOrDefault(x => x.ProjectId == order.ProjectId).Date
                where nextRelease < order.End
                select new { OrderId = order.Id, order.ProjectId, Begin = nextRelease, order.End, bid.CategoryId };

            var ruleResults =
                from req in requiredRestrictions
                let restrictionExist = query.For<Project.CostPerClickRestriction>().Any(x => x.ProjectId == req.ProjectId &&
                                                                                             x.CategoryId == req.CategoryId &&
                                                                                             x.Begin <= req.Begin && req.Begin < x.End)
                where !restrictionExist
                select new Version.ValidationResult
                {
                    MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeCategory>(req.CategoryId),
                                    new Reference<EntityTypeProject>(req.ProjectId))
                                .ToXDocument(),

                    PeriodStart = req.Begin,
                    PeriodEnd = req.End,
                    OrderId = req.OrderId,
                };

            return ruleResults;
        }
    }
}