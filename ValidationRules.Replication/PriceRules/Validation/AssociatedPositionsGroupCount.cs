using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для прайс-листов, у позиций которых более одной AssociatedPositionsGroup должно выводиться предупреждение.
    /// "В Позиции прайс-листа {0} содержится более одной группы сопутствующих позиций, что не поддерживается системой."
    /// 
    /// Source: AssociatedAndDeniedPricePositionsOrderValidationRule/InPricePositionOf_Price_ContaiedMoreThanOneAssociatedPositions
    /// </summary>
    public sealed class AssociatedPositionsGroupCount : ValidationResultAccessorBase
    {
        public AssociatedPositionsGroupCount(IQuery query) : base(query, MessageTypeCode.AssociatedPositionsGroupCount)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from overcount in query.For<Price.AssociatedPositionGroupOvercount>()
                join pp in query.For<Period.PricePeriod>() on overcount.PriceId equals pp.PriceId
                join period in query.For<Period>() on new { pp.Start, pp.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeProject>(period.ProjectId),
                                    new Reference<EntityTypePricePosition>(overcount.PricePositionId,
                                        new Reference<EntityTypePosition>(overcount.PositionId)))
                                .ToXDocument(),

                        PeriodStart = period.Start,
                        PeriodEnd = period.End,
                        ProjectId = period.ProjectId,
                    };

            return ruleResults;
        }
    }
}
