using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var firmReference = validationResult.ReadFirmReference();

            return new MessageSerializerResult(
                firmReference,
                "У фирмы {0}, с рубрикой \"Выгодные покупки с 2ГИС\", отсутствуют продажи по позициям \"Самореклама только для ПК\" или \"Выгодные покупки с 2ГИС\"",
                firmReference);
        }
    }
}