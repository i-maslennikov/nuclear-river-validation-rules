using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers.ConsistencyRules
{
    public sealed class LinkedCategoryShouldBeActiveMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public LinkedCategoryShouldBeActiveMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.LinkedCategoryShouldBeActive;

        public MessageTemplate Serialize(ValidationResult validationResult)
        {
            var orderReference = validationResult.ReadOrderReference();
            var orderPositionReference = validationResult.ReadOrderPositionReference();
            var categoryReference = validationResult.ReadCategoryReference();

            return new MessageTemplate(
                orderReference,
                "В позиции {0} найдена неактивная рубрика {1}",
                _linkFactory.CreateLink(orderPositionReference),
                _linkFactory.CreateLink(categoryReference));
        }
    }
}