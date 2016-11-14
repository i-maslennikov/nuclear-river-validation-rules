using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LegalPersonProfileWarrantyShouldNotBeExpiredMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var legalPersonProfileReference = validationResult.ReadLegalPersonProfileReference();

            return new MessageComposerResult(
                orderReference,
                "У юр. лица клиента, в профиле {0} указана доверенность с датой окончания действия раньше даты подписания заказа",
                legalPersonProfileReference);
        }
    }
}