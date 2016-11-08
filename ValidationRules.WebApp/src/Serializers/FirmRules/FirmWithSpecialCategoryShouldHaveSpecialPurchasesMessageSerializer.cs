using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.FirmRules
{
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var firmReference = validationResult.ReadFirmReference();

            return new MessageTemplate(
                firmReference,
                "У фирмы {0}, с рубрикой \"Выгодные покупки с 2ГИС\", отсутствуют продажи по позициям \"Самореклама только для ПК\" или \"Выгодные покупки с 2ГИС\"",
                _linkFactory.CreateLink(firmReference));
        }
    }
}