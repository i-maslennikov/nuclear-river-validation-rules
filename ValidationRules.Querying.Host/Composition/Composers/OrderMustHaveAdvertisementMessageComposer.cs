using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustHaveAdvertisementMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustHaveAdvertisement;

        public MessageComposerResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            var orderReference = references.Get("order");
            var advertisementReference = references.Get("advertisement");
            var advertisementElementReference = references.Get("advertisementElement");

            return new MessageComposerResult(
                orderReference,
                Resources.OrdersCheckPositionMustHaveAdvertisementElements,
                advertisementReference,
                advertisementElementReference);
        }
    }
}