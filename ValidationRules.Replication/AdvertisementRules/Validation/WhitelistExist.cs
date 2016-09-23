using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для фирмы {firm} в белый список выбран рекламный материал {advertisement}
    /// 
    /// Source: AdvertisementsOnlyWhiteListOrderValidationRule/AdvertisementChoosenForWhitelist
    /// </summary>
    public sealed class WhiteListExist : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Info)
                                                                    .WhenMass(Result.Info)
                                                                    .WhenMassPrerelease(Result.Info)
                                                                    .WhenMassRelease(Result.Info);

        public WhiteListExist(IQuery query) : base(query, MessageTypeCode.WhiteListExist)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.WhiteListExist>() on order.Id equals fail.OrderId
                              select new Version.ValidationResult
                                  {
                                  MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                  new XElement("firm",
                                                                                              new XAttribute("id", fail.FirmId),
                                                                                              new XAttribute("name", query.For<Firm>().Single(x => x.Id == fail.FirmId).Name)),
                                                                                  new XElement("advertisement",
                                                                                              new XAttribute("id", fail.AdvertisementId),
                                                                                              new XAttribute("name", query.For<Advertisement>().Single(x => x.Id == fail.AdvertisementId).Name))
                                                                                  )),
                                      PeriodStart = order.BeginDistributionDate,
                                      PeriodEnd = order.EndDistributionDatePlan,
                                      ProjectId = order.ProjectId,

                                      Result = RuleResult,
                                  };

            return ruleResults;
        }
    }
}
