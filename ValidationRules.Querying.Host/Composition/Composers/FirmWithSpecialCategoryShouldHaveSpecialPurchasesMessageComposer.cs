using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchasesMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchases;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var firmReference = validationResult.ReadFirmReference();

            return new MessageComposerResult(
                firmReference,
                "У фирмы {0}, с рубрикой \"Выгодные покупки с 2ГИС\", отсутствуют продажи по позициям \"Самореклама только для ПК\" или \"Выгодные покупки с 2ГИС\"",
                firmReference);
        }
    }
}