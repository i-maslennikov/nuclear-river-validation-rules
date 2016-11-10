using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class LinkedCategoryFirmAddressShouldBeValidMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryFirmAddressShouldBeValid;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var categoryReference = validationResult.ReadCategoryReference();
            var firmAddressReference = validationResult.ReadFirmAddressReference();

            return new MessageSerializerResult(
                orderReference,
                "В позиции {0} найдена рубрика {1}, не принадлежащая адресу {2}",
                orderPositionReference,
                categoryReference,
                firmAddressReference);
        }
    }
}