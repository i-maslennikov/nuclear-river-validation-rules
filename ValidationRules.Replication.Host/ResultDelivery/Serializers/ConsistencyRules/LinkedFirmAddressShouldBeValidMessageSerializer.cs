using System;

using NuClear.ValidationRules.Storage.Model.ConsistencyRules.Aggregates;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ConsistencyRules
{
    public sealed class LinkedFirmAddressShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedFirmAddressShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmAddressShouldBeValid;

        public LocalizedMessage Serialize(Message message)
        {
            var orderReference = message.ReadOrderReference();
            var orderPositionReference = message.ReadOrderPositionReference();
            var firmAddressReference = message.ReadFirmAddressReference();

            var firmAddressState = message.ReadFirmAddressState();
            string format;
            switch (firmAddressState)
            {
                case InvalidFirmAddressState.Deleted:
                    format = "В позиции {0} адрес фирмы {1} скрыт навсегда";
                    break;
                case InvalidFirmAddressState.NotActive:
                    format = "В позиции {0} адрес фирмы {1} неактивен";
                    break;
                case InvalidFirmAddressState.ClosedForAscertainment:
                    format = "В позиции {0} адрес фирмы {1} скрыт до выяснения";
                    break;
                case InvalidFirmAddressState.NotBelongToFirm:
                    format = "В позиции {0} адрес фирмы {1} не принадлежит фирме заказа";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new LocalizedMessage(message.GetLevel(),
                                        $"Заказ {_linkFactory.CreateLink(orderReference)}",
                                        string.Format(format, _linkFactory.CreateLink(orderPositionReference), _linkFactory.CreateLink(firmAddressReference)));
        }
    }
}