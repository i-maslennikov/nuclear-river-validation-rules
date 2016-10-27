using System;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class LinkedFirmShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedFirmShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmShouldBeValid;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();

            var firmState = validationResult.ReadFirmState();
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

            return new MessageTemplate(
                orderReference,
                format,
                _linkFactory.CreateLink(firmReference));
        }
    }
}