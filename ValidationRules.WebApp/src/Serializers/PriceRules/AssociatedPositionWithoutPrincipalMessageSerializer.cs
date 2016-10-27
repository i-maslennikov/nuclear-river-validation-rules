using System.Linq;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.PriceRules
{
    public sealed class AssociatedPositionWithoutPrincipalMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public AssociatedPositionWithoutPrincipalMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.AssociatedPositionWithoutPrincipal;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var position = validationResult.ReadOrderPositions().First();

            if (position.OrderPositionName != position.PositionName)
            {
                return new MessageTemplate(orderReference,
                                           "Подпозиция {0} позиции {1} является сопутствующей, основная позиция не найдена",
                                           position.PositionName,
                                           _linkFactory.CreateLink("OrderPosition", position.OrderPositionId, position.OrderPositionName));
            }
            else
            {
                return new MessageTemplate(orderReference,
                                           "Позиция {0} является сопутствующей, основная позиция не найдена",
                                           _linkFactory.CreateLink("OrderPosition", position.OrderPositionId, position.OrderPositionName));
            }
        }
    }
}