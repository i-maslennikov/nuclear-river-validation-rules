using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrderMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.FirmWithSpecialCategoryShouldHaveSpecialPurchasesOrder;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var firmReference = validationResult.ReadFirmReference();

            return new MessageComposerResult(
                orderReference,
                "У фирмы {0}, с рубрикой \"Выгодные покупки с 2ГИС\", отсутствуют продажи по позициям \"Самореклама только для ПК\" или \"Выгодные покупки с 2ГИС\"",
                firmReference);
        }
    }
}