using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmAddressMayBeSharedWithPartnerMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmAddressMayBeSharedWithPartner;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            // два заказа: в первом выводим предупреждение (это заказ фирмы, чей адрес), второй - подкидывает рекламу этой фирме
            var orderReferences = references.GetMany<EntityTypeOrder>().ToArray();
            var firmReference = references.Get<EntityTypeFirm>();
            var addressReference = references.Get<EntityTypeFirmAddress>();

            return new MessageComposerResult(
                orderReferences[0],
                Resources.FirmAddressMayBeSharedWithPartner,
                addressReference,
                firmReference,
                orderReferences[1]);
        }
    }
}