using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Replication.AdvertisementRules.Validation
{
    /// <summary>
    /// Для заказов, у которых текст рекламного материала повторяет контактную ссылку фирмы, должно выводиться предупреждение
    /// 
    /// Для фирмы {0} заказана рекламная ссылка {1} позиция {2}, дублирующая контакт фирмы
    /// 
    /// Source: ContactDoesntContainSponsoredLinkOrderValidationRule/FirmContactContainsSponsoredLinkError
    /// </summary>
    public sealed class AdvertisementWebsiteShouldNotBeFirmWebsite : ValidationResultAccessorBase
    {
        public AdvertisementWebsiteShouldNotBeFirmWebsite(IQuery query) : base(query, MessageTypeCode.AdvertisementWebsiteShouldNotBeFirmWebsite)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var ruleResults =
                from order in query.For<Order>()
                from firmWebSite in query.For<Firm.FirmWebsite>().Where(x => x.FirmId == order.FirmId)
                from opa in query.For<Order.OrderPositionAdvertisement>().Where(x => x.OrderId == order.Id)
                from advertisementWebsite in query.For<Advertisement.AdvertisementWebsite>().Where(x => x.AdvertisementId == opa.AdvertisementId)
                where advertisementWebsite.Website.Contains(firmWebSite.Website) // рекламная ссылка начинается также как контактная
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                    new Dictionary<string, object> { { "website", advertisementWebsite.Website } },
                                    new Reference<EntityTypeOrder>(order.Id),
                                    new Reference<EntityTypeFirm>(order.FirmId),
                                    new Reference<EntityTypeOrderPositionAdvertisement>(0,
                                        new Reference<EntityTypeOrderPosition>(opa.OrderPositionId),
                                        new Reference<EntityTypePosition>(opa.PositionId)))
                                .ToXDocument(),

                        PeriodStart = order.BeginDistributionDate,
                        PeriodEnd = order.EndDistributionDatePlan,
                        OrderId = order.Id,
                    };

            return ruleResults;
        }
    }
}
