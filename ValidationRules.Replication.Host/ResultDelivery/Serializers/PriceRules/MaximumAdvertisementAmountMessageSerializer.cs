﻿namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.PriceRules
{
    public sealed class MaximumAdvertisementAmountMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public MaximumAdvertisementAmountMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.MaximumAdvertisementAmount;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var dto = message.ReadAdvertisementCountMessage();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        $"Позиция {dto.Name} должна присутствовать в сборке в количестве от {dto.Min} до {dto.Max}. Фактическое количество позиций в месяц {dto.Month:d} - {dto.Count}");
        }
    }
}