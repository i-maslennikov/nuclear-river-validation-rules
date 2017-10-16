using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для заказов, в карточках фирмы которых размещаются позиции 'Реклама в профилях партнёров', должно выводиться предупреждение.
    /// "На адрес {0} продана позиция 'Реклама в профилях партнёров' из заказа {1} фирмы {2}"
    /// 
    /// Является зеркальным отражением проверки <see cref="PartnerAdvertisementShouldNotBeSoldToAdvertiser"/>
    /// </summary>
    public sealed class AdvertiserMustBeNotifiedAboutPartnerAdvertisement : ValidationResultAccessorBase
    {
        public AdvertiserMustBeNotifiedAboutPartnerAdvertisement(IQuery query) : base(query, MessageTypeCode.AdvertiserMustBeNotifiedAboutPartnerAdvertisement)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages =
                from order in query.For<Order>()
                from partnerPosition in query.For<Order.PartnerPosition>().Where(x => x.DestinationFirmId == order.FirmId)
                select new Version.ValidationResult
                    {
                        MessageParams =
                            new MessageParams(
                                              new Reference<EntityTypeOrder>(order.Id), // Заказ фирмы-рекламодателя (хоста)
                                              new Reference<EntityTypeOrder>(partnerPosition.OrderId), // Заказ, размещающий ссылку
                                              new Reference<EntityTypeFirm>(partnerPosition.DestinationFirmId),
                                              new Reference<EntityTypeFirmAddress>(partnerPosition.DestinationFirmAddressId))
                                .ToXDocument(),

                        PeriodStart = order.Begin,
                        PeriodEnd = order.End,
                        OrderId = order.Id,
                    };

            return messages;
        }
    }
}
