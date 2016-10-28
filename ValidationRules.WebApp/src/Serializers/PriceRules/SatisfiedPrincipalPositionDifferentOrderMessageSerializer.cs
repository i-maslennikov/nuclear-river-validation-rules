using System.Linq;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class SatisfiedPrincipalPositionDifferentOrderMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public SatisfiedPrincipalPositionDifferentOrderMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.SatisfiedPrincipalPositionDifferentOrder;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositions = validationResult.ReadOrderPositions();

            var dependentPositions = orderPositions.Skip(1).Select(x => $"{MakePositionText(x)} Заказа {_linkFactory.CreateLink("Order", x.OrderId, x.OrderNumber)}");

            return new MessageTemplate(
                orderReference,
                "{0} данного Заказа является основной для следующих позиций: {1}",
                MakePositionText(orderPositions.First()),
                string.Join(", ", dependentPositions));
        }

        private string MakePositionText(MessageExtensions.OrderPositionDto dto)
        {
            return dto.OrderPositionName != dto.PositionName
                       ? $"Подпозиция {dto.PositionName} позиции {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}"
                       : $"Позиция {_linkFactory.CreateLink("OrderPosition", dto.OrderPositionId, dto.OrderPositionName)}";
        }
    }
}