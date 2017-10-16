using System.Linq;

using NuClear.Storage.API.Readings;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Replication.FirmRules.Validation
{
    /// <summary>
    /// Для заказов, размещающих позиции 'Реклама в профилях партнёров' в карточках фирм-рекламодателей, должно выводиться предупреждение.
    /// "Адрес {0} принадлежит фирме-рекламодателю {1} с заказом {2}"
    /// 
    /// Является зеркальным отражением проверки <see cref="AdvertiserMustBeNotifiedAboutPartnerAdvertisement"/>
    /// </summary>
    public sealed class PartnerAdvertisementShouldNotBeSoldToAdvertiser : ValidationResultAccessorBase
    {
        public PartnerAdvertisementShouldNotBeSoldToAdvertiser(IQuery query) : base(query, MessageTypeCode.PartnerAdvertisementShouldNotBeSoldToAdvertiser)
        {
        }

        protected override IQueryable<Version.ValidationResult> GetValidationResults(IQuery query)
        {
            var messages =
                from order in query.For<Order>()
                from partnerPosition in query.For<Order.PartnerPosition>().Where(x => x.DestinationFirmId == order.FirmId)
                from partnerOrder in query.For<Order>().Where(x => x.Id == partnerPosition.OrderId)
                select new Version.ValidationResult
                {
                    MessageParams =
                            new MessageParams(
                                              new Reference<EntityTypeOrder>(order.Id), // Заказ фирмы-рекламодателя (хоста)
                                              new Reference<EntityTypeOrder>(partnerOrder.Id), // Заказ, размещающий ссылку
                                              new Reference<EntityTypeFirm>(partnerPosition.DestinationFirmId),
                                              new Reference<EntityTypeFirmAddress>(partnerPosition.DestinationFirmAddressId))
                                .ToXDocument(),

                    PeriodStart = partnerOrder.Begin,
                    PeriodEnd = partnerOrder.End,
                    OrderId = partnerOrder.Id,
                };

            return messages;
        }
    }
}
