using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class DeniedPositionsCheckMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.DeniedPositionsCheck;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var positions = validationResult.ReadOrderPositions().OrderBy(x => x.OrderPositionId);
            var first = positions.First();
            var second = positions.Last();

            return new MessageSerializerResult(
                orderReference,
                $"{MakePositionText(first)} {{0}} является запрещённой для: {MakePositionText(second)} {{1}}  в заказе {2}",
                new EntityReference("OrderPosition", first.OrderPositionId, first.OrderPositionName),
                new EntityReference("OrderPosition", second.OrderPositionId, second.OrderPositionName),
                new EntityReference("Order", second.OrderId, second.OrderNumber));
        }

        private string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции"
                       : $"Позиция";
        }
    }
}