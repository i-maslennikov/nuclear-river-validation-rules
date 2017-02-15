using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrderMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var firmReference = references.Get<EntityTypeFirm>();

            return new MessageComposerResult(
                orderReference,
                Resources.ThereIsNoAdvertisementForAdvantageousPurchasesCategory,
                firmReference);
        }
    }
}