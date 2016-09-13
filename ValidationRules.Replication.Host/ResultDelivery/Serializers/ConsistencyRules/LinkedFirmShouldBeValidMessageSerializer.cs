using System;

using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LinkedFirmShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedFirmShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmShouldBeValid;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var firmReference = message.ReadFirmReference();

            var firmState = message.ReadFirmState();
            string format;
            switch (firmState)
            {
                case InvalidFirmState.Deleted:
                    format = "Фирма {0} удалена";
                    break;
                case InvalidFirmState.ClosedForever:
                    format = "Фирма {0} скрыта навсегда";
                    break;
                case InvalidFirmState.ClosedForAscertainment:
                    format = "Фирма {0} скрыта до выяснения";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        string.Format(format, _linkFactory.CreateLink(firmReference)));
        }
    }
}