using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public class ValidationResultFactory
    {
        private readonly Dictionary<MessageTypeCode, IMessageComposer> _serializers;

        public ValidationResultFactory(IReadOnlyCollection<IMessageComposer> serializers)
        {
            _serializers = serializers.ToDictionary(x => x.MessageType, x => x);
        }

        public IReadOnlyCollection<ValidationResult> ComposeAll(IReadOnlyCollection<Version.ValidationResult> messages, Func<Version.ValidationResult, Result> selector)
            => messages.Select(x => Compose(x, selector)).ToArray();

        private ValidationResult Compose(Version.ValidationResult message, Func<Version.ValidationResult, Result> selector)
        {
            var x = Compose(message);

            return new ValidationResult
            {
                MainReference = x.MainReference,
                Template = x.Template,
                References = x.References,
                Result = selector(message),
                Rule = message.MessageType,
            };
        }

        private MessageComposerResult Compose(Version.ValidationResult message)
        {
            IMessageComposer composer;
            if (!_serializers.TryGetValue((MessageTypeCode)message.MessageType, out composer))
            {
                throw new ArgumentException($"Не найден сериализатор '{message.MessageType}'", nameof(message));
            }

            try
            {
                return composer.Serialize(message);
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