using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class PremiumPartnerProfileMustHaveSingleSaleMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.PremiumPartnerProfileMustHaveSingleSale;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var firmReference = references.Get<EntityTypeFirm>();
            var addressReference = references.Get<EntityTypeFirmAddress>();

            return new MessageComposerResult(
                orderReference,
                string.Format(Resources.MoreThanOnePremiumBuyHerePositionSoldForAddress, string.Join(", ", extra.Keys)),
                addressReference,
                firmReference);
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
            => messages
                .GroupBy(x => new { x.OrderId, FirmAddressId = x.References.Get<EntityTypeFirmAddress>().Id, FirmId = x.References.Get<EntityTypeFirm>().Id, x.MessageType, x.ProjectId },
                         x => x)
                .Select(group => new Message
                    {
                        OrderId = group.Key.OrderId,
                        MessageType = group.Key.MessageType,
                        ProjectId = group.Key.ProjectId,
                        References = new Reference[] { new Reference<EntityTypeFirmAddress>(group.Key.FirmAddressId), new Reference<EntityTypeFirm>(group.Key.FirmId) },
                        Extra = group.SelectMany(x => MonthlySplit(DateTime.Parse(x.Extra["begin"]), DateTime.Parse(x.Extra["end"])))
                                     .Distinct()
                                     .OrderBy(x => x)
                                     .ToDictionary(x => x.ToString("MMMM yyyy"), x => string.Empty),
                    });

        private static IEnumerable<DateTime> MonthlySplit(DateTime begin, DateTime end)
        {
            for (var x = begin; x < end; x = x.AddMonths(1))
            {
                yield return x;
            }
        }
    }
}