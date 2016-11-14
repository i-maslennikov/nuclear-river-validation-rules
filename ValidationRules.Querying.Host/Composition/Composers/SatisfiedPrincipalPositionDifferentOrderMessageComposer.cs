using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class SatisfiedPrincipalPositionDifferentOrderMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder;

        public MessageComposerResult Serialize(Version.ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositions = validationResult.ReadOrderPositions();

            var master = orderPositions.First();
            var dependents = orderPositions.Skip(1).ToArray();
            var placeholders = dependents.Select((x, i) => $"{MakePositionText(x)} {{{1 + i}}} Заказа {{{1 + dependents.Length + i}}}");

            return new MessageComposerResult(
                orderReference,
                $"{MakePositionText(master)} {{0}} данного Заказа является основной для следующих позиций: {string.Join(", ", placeholders)}",
                new[] { new EntityReference("OrderPosition", master.OrderPositionId, master.OrderPositionName) }
                    .Concat(dependents.Select(x => new EntityReference("OrderPosition", x.OrderPositionId, x.OrderPositionName)))
                    .Concat(dependents.Select(x => new EntityReference("Order", x.OrderId, x.OrderNumber)))
                    .ToArray());
        }

        private string MakePositionText(ResultExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции"
                       : $"Позиция";
        }
    }
}