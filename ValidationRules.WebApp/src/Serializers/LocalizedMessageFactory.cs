using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.WebApp.Entity;

namespace NuClear.ValidationRules.WebApp.Serializers
{
    public sealed class LocalizedMessageFactory
    {
        private static readonly IReadOnlyCollection<Type> SerializerTypes =
            typeof(LocalizedMessageFactory).Assembly.GetTypes().Where(x => typeof(IMessageSerializer).IsAssignableFrom(x) && x.IsClass && !x.IsAbstract).ToArray();

        private readonly Dictionary<MessageTypeCode, IMessageSerializer> _serializers;

        public LocalizedMessageFactory(LinkFactory linkFactory)
        {
            var args = new object[] { linkFactory };
            _serializers = SerializerTypes.Select(x => Activator.CreateInstance(x, args)).Cast<IMessageSerializer>().ToDictionary(x => x.MessageType, x => x);
        }

        public MessageTemplate Localize(ValidationResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            try
            {
                IMessageSerializer serializer;
                if (!_serializers.TryGetValue((MessageTypeCode)result.MessageType, out serializer))
                {
                    return new MessageTemplate(Tuple.Create("Order", 0L, "Неизвестный заказ"), "Неизвестная ошибка с кодом {0}. Обратитесь к разработчикам.", result.MessageType) { MessageType = result.MessageType };
                }

                var res = serializer.Serialize(result);
                res.MessageType = result.MessageType;
                return res;
            }
            catch (Exception ex)
            {
                return new MessageTemplate(Tuple.Create("Order", 0L, "Неизвестный заказ"), "Не удалось прочитать ошибку: {0} {1}. Обратитесь к разработчикам.", result.MessageType, ex.Message) { MessageType = 255 };
            }
        }
    }
}
