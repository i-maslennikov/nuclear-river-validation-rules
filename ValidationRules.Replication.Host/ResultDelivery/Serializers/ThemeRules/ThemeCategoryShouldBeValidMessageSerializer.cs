namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ThemeRules
{
    public sealed class ThemeCategoryShouldBeValidMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public ThemeCategoryShouldBeValidMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.ThemeCategoryShouldBeValid;

        public LocalizedMessage Serialize(Message message)
        {
            var themeReference = message.ReadThemeReference();
            var categoryReference = message.ReadCategoryReference();

            return new LocalizedMessage(message.GetLevel(),
                                        $"Тематика {_linkFactory.CreateLink(themeReference)}",
                                        $"Тематика {_linkFactory.CreateLink(themeReference)} использует удаленную рубрику {_linkFactory.CreateLink(categoryReference)}");
        }
    }
}