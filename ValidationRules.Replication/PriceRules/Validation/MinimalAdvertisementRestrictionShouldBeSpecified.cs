using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.Messages;
using NuClear.ValidationRules.Storage.Model.PriceRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.PriceRules.Validation
{
    /// <summary>
    /// Для прайс-листов, в которых для позиций с контроллируемым количеством не указан минимум должна выводиться ошибка.
    /// "В позиции прайса {0} необходимо указать минимальное количество рекламы в выпуск"
    /// 
    /// Source: AdvertisementAmountOrderValidationRule/PricePositionHasNoMinAdvertisementAmount
    /// Ошибка на самом деле выводится не для позиции прайса, а для номенклатурной позиции в прайсе, но такой сущности попросту нет.
    /// </summary>
    public sealed class MinimalAdvertisementRestrictionShouldBeSpecified : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.None)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public MinimalAdvertisementRestrictionShouldBeSpecified(IQuery query) : base(query, MessageTypeCode.MinimalAdvertisementRestrictionShouldBeSpecified)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from restriction in query.For<Price.AdvertisementAmountRestriction>().Where(x => x.MissingMinimalRestriction)
                              join pp in query.For<Period.PricePeriod>() on restriction.PriceId equals pp.PriceId
                              join period in query.For<Period>() on new { pp.Start, pp.OrganizationUnitId } equals new { period.Start, period.OrganizationUnitId }
                              join op in query.For<Period.OrderPeriod>() on new { pp.Start, pp.OrganizationUnitId } equals new { op.Start, op.OrganizationUnitId }
                              select new Version.ValidationResult
                                  {
                                      MessageParams = new XDocument(new XElement("root",
                                          new XElement("project",
                                              new XAttribute("id", period.ProjectId)),
                                          new XElement("pricePosition",
                                              new XAttribute("name", restriction.CategoryName)))),

                                      PeriodStart = period.Start,
                                      PeriodEnd = period.End,
                                      ProjectId = period.ProjectId,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
