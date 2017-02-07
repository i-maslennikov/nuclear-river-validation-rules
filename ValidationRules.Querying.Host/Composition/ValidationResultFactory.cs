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
        private static readonly IDistinctor Default = new DefaultDistinctor();

        private readonly IReadOnlyDictionary<MessageTypeCode, IMessageComposer> _composers;
        private readonly IReadOnlyDictionary<MessageTypeCode, IDistinctor> _distinctors;

        public ValidationResultFactory(IReadOnlyCollection<IMessageComposer> composers, IReadOnlyCollection<IDistinctor> distinctors)
        {
            _composers = composers.ToDictionary(x => x.MessageType, x => x);
            _distinctors = distinctors.ToDictionary(x => x.MessageType, x => x);
        }

        public IReadOnlyCollection<ValidationResult> ComposeAll(IEnumerable<Version.ValidationResult> messages, Func<CombinedResult, Result> selector)
            => MakeDistinct(messages).Select(x => Compose(x, selector)).Where(x => x != null).ToArray();

        private ValidationResult Compose(Version.ValidationResult message, Func<CombinedResult, Result> selector)
        {
            var x = Compose(message);
            var result = selector.Invoke(CombinedResult.FromInt32(message.Result));
            if (result == Result.None)
            {
                return null;
            }

            return new ValidationResult
            {
                MainReference = x.MainReference,
                Template = x.Template,
                References = x.References,
                Result = result,
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

        private IEnumerable<Version.ValidationResult> MakeDistinct(IEnumerable<Version.ValidationResult> messages)
            => messages.GroupBy(x => (MessageTypeCode)x.MessageType)
                       .SelectMany(x => DistinctorForMessageType(x.Key).Distinct(x));

        private IDistinctor DistinctorForMessageType(MessageTypeCode messageType)
        {
            IDistinctor distinctor;
            return _distinctors.TryGetValue(messageType, out distinctor) ? distinctor : Default;
        }

        class DefaultDistinctor : IDistinctor
        {
            public MessageTypeCode MessageType => 0;

            public IEnumerable<Version.ValidationResult> Distinct(IEnumerable<Version.ValidationResult> results)
                => results;
        }
    }
}