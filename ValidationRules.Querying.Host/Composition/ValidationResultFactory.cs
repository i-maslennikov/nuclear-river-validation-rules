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
        private readonly Dictionary<MessageTypeCode, IMessageComposer> _composers;

        public ValidationResultFactory(IReadOnlyCollection<IMessageComposer> composers)
        {
            _composers = composers.ToDictionary(x => x.MessageType, x => x);
        }

        public IReadOnlyCollection<ValidationResult> ComposeAll(IEnumerable<Version.ValidationResult> messages, Func<CombinedResult, Result> selector)
            => messages.Select(x => Compose(x, selector)).ToArray();

        private ValidationResult Compose(Version.ValidationResult message, Func<CombinedResult, Result> selector)
        {
            var x = Compose(message);

            return new ValidationResult
            {
                MainReference = x.MainReference,
                Template = x.Template,
                References = x.References,
                Result = selector.Invoke(CombinedResult.FromInt32(message.Result)),
                Rule = message.MessageType,
            };
        }

        private MessageComposerResult Compose(Version.ValidationResult message)
        {
            IMessageComposer composer;
            if (!_composers.TryGetValue((MessageTypeCode)message.MessageType, out composer))
            {
                throw new ArgumentException($"Не найден сериализатор '{message.MessageType}'", nameof(message));
            }

            try
            {
                return composer.Compose(message);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сериализации сообщения {message.MessageType}", ex);
            }
        }
    }
}