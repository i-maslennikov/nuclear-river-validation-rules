using System;

namespace NuClear.ValidationRules.Replication.Host.ResultDelivery.Serializers.ThemeRules
{
    public sealed class DefaultThemeMustBeExactlyOneMessageSerializer : IMessageSerializer
    {
        private readonly LinkFactory _linkFactory;

        public DefaultThemeMustBeExactlyOneMessageSerializer(LinkFactory linkFactory)
        {
            _linkFactory = linkFactory;
        }

        public MessageTypeCode MessageType => MessageTypeCode.DefaultThemeMustBeExactlyOne;

        public LocalizedMessage Serialize(Message message)
        {
            var projectReference = message.ReadProjectReference();
            var themeCount = message.ReadProjectThemeCount();

            string error;
            if (themeCount == 0)
            {
                error = $"Для подразделения {_linkFactory.CreateLink(projectReference)} не указана тематика по умолчанию";
            } else if (themeCount > 1)
            {
                error = $"Для подразделения {_linkFactory.CreateLink(projectReference)} установлено более одной тематики по умолчанию";
            }
            else
            {
                throw new ArgumentException();
            }

            return new LocalizedMessage(message.GetLevel(),
                                        $"Проект {_linkFactory.CreateLink(projectReference)}",
                                        error);
        }
    }
}