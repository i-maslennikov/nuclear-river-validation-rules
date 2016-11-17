using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class AssociatedPositionWithoutPrincipalMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionWithoutPrincipal;

        public MessageComposerResult Compose(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var position = validationResult.ReadOrderPositions().First();

            return new MessageComposerResult(
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