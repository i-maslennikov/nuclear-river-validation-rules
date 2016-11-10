using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class AssociatedPositionsGroupCountMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionsGroupCount;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var price = validationResult.ReadPriceReference();
            var pricePosition = validationResult.ReadPricePositionReference();

            return new MessageSerializerResult(
                price,
                "В Позиции прайс-листа {0} содержится более одной группы сопутствующих позиций, что не поддерживается системой",
                pricePosition);
        }
    }
}