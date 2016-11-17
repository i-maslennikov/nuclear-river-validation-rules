using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LegalPersonProfileBargainShouldNotBeExpiredMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonProfileBargainShouldNotBeExpired;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var legalPersonProfileReference = validationResult.ReadLegalPersonProfileReference();

            return new MessageComposerResult(
                orderReference,
                "У юр. лица клиента, в профиле {0} указан договор с датой окончания действия раньше даты подписания заказа",
                legalPersonProfileReference);
        }
    }
}