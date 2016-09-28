using System.Linq;
using System.Xml.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для фирм, у которых недостаёт выбранных в белый список РМ, должна выводиться ошибка при массовой проверке и предупреждение при единичной.
    /// "Для фирмы {0} не указан рекламный материал в белый список"
    /// 
    /// Source: AdvertisementsOnlyWhiteListOrderValidationRule/AdvertisementForWhitelistDoesNotSpecified
    /// 
    /// * Поскольку проверок фирм нет, то сообщения выводим в заказах этой фирмы, в которых есть как минимум один РМ с возможностью выбора в белый список.
    /// * "Недостаёт" - значит, в выпуск выходит как минимум один РМ с возможностью выбора в белый список, но ни одного выбранного.
    /// </summary>
    public sealed class RequiredWhiteListMissing : ValidationResultAccessorBase
    {
        private static readonly int RuleResult = new ResultBuilder().WhenSingle(Result.Warning)
                                                                    .WhenMass(Result.Warning)
                                                                    .WhenMassPrerelease(Result.Error)
                                                                    .WhenMassRelease(Result.Error);

        public RequiredWhiteListMissing(IQuery query) : base(query, MessageTypeCode.RequiredWhiteListMissing)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults = from order in query.For<Order>()
                              join fail in query.For<Order.WhiteListAdvertisement>() on order.Id equals fail.OrderId
                              where fail.AdvertisementId == null
                              select new Version.ValidationResult
                                  {
                                  MessageParams = new XDocument(new XElement("root",
                                                                                 new XElement("order",
                                                                                              new XAttribute("id", order.Id),
                                                                                              new XAttribute("number", order.Number)),
                                                                                  new XElement("firm",
                                                                                              new XAttribute("id", fail.FirmId),
                                                                                              new XAttribute("name", query.For<Firm>().Single(x => x.Id == fail.FirmId).Name))
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
