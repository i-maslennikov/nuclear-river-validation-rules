using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class AssociatedPositionWithoutPrincipalMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionWithoutPrincipal;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var position = validationResult.ReadOrderPositions().First();

            return new MessageSerializerResult(
                orderReference,
                $"{MakePositionText(position)} {{0}} является сопутствующей, основная позиция не найдена",
                new EntityReference("OrderPosition", position.OrderPositionId, position.OrderPositionName));
        }

        private string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции"
                       : $"Позиция";
        }
    }
}