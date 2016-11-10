using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class LegalPersonShouldHaveAtLeastOneProfileMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonShouldHaveAtLeastOneProfile;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(
                orderReference,
                "У юр. лица клиента отсутствует профиль");
        }
    }
}