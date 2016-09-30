using System.Collections.Generic;

using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class AdvertisementElementMustPassReviewMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementElementMustPassReviewMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementElementMustPassReview;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var advertisementElementReference = message.ReadAdvertisementElementReference();
            var advertisementElementStatus = message.ReadAdvertisementElementStatus();

            var status = new Dictionary<Advertisement.ReviewStatus, string>
                {
                    { Advertisement.ReviewStatus.Invalid, "содержит ошибки выверки"},
                    { Advertisement.ReviewStatus.Draft, "находится в статусе 'Черновик'"},
                };

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"В рекламном материале {_linkFactory.CreateLink(advertisementReference)}, который подлежит выверке, элемент {_linkFactory.CreateLink(advertisementElementReference)} {status[advertisementElementStatus]}");
        }
    }
}