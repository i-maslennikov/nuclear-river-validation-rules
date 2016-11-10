using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Serialization
{
    public class MessageSerializer
    {
        private readonly Dictionary<MessageTypeCode, IMessageSerializer> _serializers;

        public MessageSerializer(IReadOnlyCollection<IMessageSerializer> serializers)
        {
            _serializers = serializers.ToDictionary(x => x.MessageType, x => x);
        }

        public IReadOnlyCollection<ValidationResult> Serialize(IReadOnlyCollection<Version.ValidationResult> messages, Func<Version.ValidationResult, Result> selector)
            => messages.Select(x => Serialize(x, selector)).ToArray();

        private ValidationResult Serialize(Version.ValidationResult message, Func<Version.ValidationResult, Result> selector)
        {
            var x = Serialize(message);

            return new ValidationResult
            {
                MainReference = x.MainReference,
                Template = x.Template,
                References = x.References,
                Result = selector(message),
                Rule = message.MessageType,
            };
        }

        private MessageSerializerResult Serialize(Version.ValidationResult message)
        {
            IMessageSerializer serializer;
            if (!_serializers.TryGetValue((MessageTypeCode)message.MessageType, out serializer))
            {
                throw new ArgumentException($"Не найден сериализатор '{message.MessageType}'", nameof(message));
            }

            try
            {
                return serializer.Serialize(message);
            }
            catch (Exception ex)
            {
                var x = new Exception("Ошибка при сериализации сообщения", ex);
                x.Data.Add("xml", message.MessageParams);
                throw x;
            }
        }
    }
}