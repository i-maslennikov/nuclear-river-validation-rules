using System.Linq;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class CouponMustBeSoldOnceAtTimeMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public CouponMustBeSoldOnceAtTimeMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.CouponMustBeSoldOnceAtTime;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var orderPositionReferences = message.ReadOrderPositionReferences();

            return new MessageTemplate(orderReference,
                "Купон на скидку {0} прикреплён к нескольким позициям: {1}",
                _linkFactory.CreateLink(advertisementReference),
                string.Join(", ", orderPositionReferences.Select(_linkFactory.CreateLink)));
        }
    }
}