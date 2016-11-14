using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LegalPersonShouldHaveAtLeastOneProfileMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageComposerResult(
                orderReference,
                "У юр. лица клиента отсутствует профиль");
        }
    }
}