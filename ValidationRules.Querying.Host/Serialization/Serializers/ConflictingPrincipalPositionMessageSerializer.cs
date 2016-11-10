using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class ConflictingPrincipalPositionMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.ConflictingPrincipalPosition;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var positions = validationResult.ReadOrderPositions();

            var first = positions.First();
            var second = positions.Last();

            return new MessageSerializerResult(
                orderReference,
                $"{MakePositionText(first)} {{0}} содержит объекты привязки, конфликтующие с объектами привязки следующей основной позиции: {MakePositionText(second)} {{1}}",
                new EntityReference("OrderPosition", first.OrderPositionId, first.OrderPositionName),
                new EntityReference("OrderPosition", second.OrderPositionId, second.OrderPositionName));
        }

        private string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции"
                       : $"Позиция";
        }
    }
}