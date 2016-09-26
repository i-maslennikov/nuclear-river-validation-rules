using System;

using NuClear.ValidationRules.Storage.Model.AdvertisementRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.AdvertisementRules
{
    public sealed class AdvertisementElementShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AdvertisementElementShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AdvertisementElementShouldBeValid;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var advertisementReference = message.ReadAdvertisementReference();
            var advertisementElementReference = message.ReadAdvertisementElementReference();
            var advertisementElementStatus = message.ReadAdvertisementElementStatus();

            switch (advertisementElementStatus)
            {
                case Advertisement.InvalidAdvertisementElementStatus.Invalid:
                    return new LocalizedMessage(message.GetLevel(),
                                                $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                                $"В рекламном материале {_linkFactory.CreateLink(advertisementReference)}, который подлежит выверке, элемент {_linkFactory.CreateLink(advertisementElementReference)} содержит ошибки выверки");
                case Advertisement.InvalidAdvertisementElementStatus.Draft:
                    return new LocalizedMessage(message.GetLevel(),
                                                $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                                $"В рекламном материале {_linkFactory.CreateLink(advertisementReference)}, который подлежит выверке, элемент {_linkFactory.CreateLink(advertisementElementReference)} находится в статусе 'Черновик'");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}