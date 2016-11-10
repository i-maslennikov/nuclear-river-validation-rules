using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class MaximumAdvertisementAmountMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.MaximumAdvertisementAmount;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var dto = validationResult.ReadAdvertisementCountMessage();

            return new MessageSerializerResult(
                orderReference,
                $"Позиция {dto.Name} должна присутствовать в сборке в количестве от {dto.Min} до {dto.Max}. Фактическое количество позиций в месяц {dto.Month:d} - {dto.Count}");
        }
    }
}