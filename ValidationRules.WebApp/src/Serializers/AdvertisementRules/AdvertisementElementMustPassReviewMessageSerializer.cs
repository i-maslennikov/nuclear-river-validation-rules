using System.Collections.Generic;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.AdvertisementRules
{
    public sealed class AdvertisementElementMustPassReviewMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementElementMustPassReviewMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementElementMustPassReview;

        public MessageTemplate Serialize(ValidationResult message)
        {
            var orderReference = message.ReadOrderReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var advertisementElementReference = message.ReadAdvertisementElementReference();
            var advertisementElementStatus = message.ReadAdvertisementElementStatus();

            var status = new Dictionary<ReviewStatus, string>
                {
                    { ReviewStatus.Invalid, "содержит ошибки выверки"},
                    { ReviewStatus.Draft, "находится в статусе 'Черновик'"},
                };

            return new MessageTemplate(orderReference,
                "В рекламном материале \"{0}\", который подлежит выверке, элемент \"{1}\" {2}",
                _linkFactory.CreateLink(advertisementReference),
                _linkFactory.CreateLink(advertisementElementReference),
                status[advertisementElementStatus]);
        }
    }
}