using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderPeriodMustContainAdvertisementPeriodMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPeriodMustContainAdvertisementPeriod;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var advertisementReference = validationResult.ReadAdvertisementReference();

            return new MessageSerializerResult(
                orderReference,
                "Период размещения рекламного материала {0}, выбранного в позиции {1} должен захватывать 5 дней от текущего месяца размещения",
                advertisementReference,
                orderPositionReference);
        }
    }
}