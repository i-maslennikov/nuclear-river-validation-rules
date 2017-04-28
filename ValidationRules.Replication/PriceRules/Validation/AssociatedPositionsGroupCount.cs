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
            var messages =
                from overcount in query.For<Price.AssociatedPositionGroupOvercount>()
                join pp in query.For<Price.PricePeriod>() on overcount.PriceId equals pp.PriceId
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Reference<EntityTypeProject>(pp.ProjectId),
                                    new Reference<EntityTypePricePosition>(overcount.PricePositionId,
                                        new Reference<EntityTypePosition>(overcount.PositionId)))
                                .ToXDocument(),

                        PeriodStart = pp.Begin,
                        PeriodEnd = pp.End,
                        ProjectId = pp.ProjectId,
                    };

            return messages;
        }
    }
}
