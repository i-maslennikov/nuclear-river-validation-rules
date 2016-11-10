using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Serialization.Serializers
{
    public sealed class LinkedObjectsMissedInPrincipalsMessageSerializer : IMessageSerializer
    {
        public MessageTypeCode MessageType => MessageTypeCode.LinkedObjectsMissedInPrincipals;

        public MessageSerializerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositions = validationResult.ReadOrderPositions();

            var first = orderPositions.First();

            return new MessageSerializerResult(
                orderReference,
                $"{MakePositionText(first)} {{0}} содержит объекты привязки, отсутствующие в основных позициях",
                new EntityReference("OrderPosition", first.OrderPositionId, first.OrderPositionName));
        }

        private string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции"
                       : $"Позиция";
        }
    }
}