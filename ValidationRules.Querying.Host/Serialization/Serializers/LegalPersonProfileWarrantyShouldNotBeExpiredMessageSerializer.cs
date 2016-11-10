using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class LegalPersonProfileWarrantyShouldNotBeExpiredMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LegalPersonProfileWarrantyShouldNotBeExpired;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var legalPersonProfileReference = validationResult.ReadLegalPersonProfileReference();

            return new MessageSerializerResult(
                orderReference,
                "У юр. лица клиента, в профиле {0} указана доверенность с датой окончания действия раньше даты подписания заказа",
                legalPersonProfileReference);
        }
    }
}