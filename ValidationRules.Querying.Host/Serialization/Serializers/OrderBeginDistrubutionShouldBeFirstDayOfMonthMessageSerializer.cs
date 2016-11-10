using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class OrderBeginDistrubutionShouldBeFirstDayOfMonthMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderBeginDistrubutionShouldBeFirstDayOfMonth;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();

            return new MessageSerializerResult(
                orderReference,
                "Указана некорректная дата начала размещения");
        }
    }
}