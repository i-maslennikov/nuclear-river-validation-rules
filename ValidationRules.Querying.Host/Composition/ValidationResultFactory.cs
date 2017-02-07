using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public class ValidationResultFactory
    {
        private readonly EntityReferenceParser _entityReferenceParser;
        private readonly Dictionary<MessageTypeCode, IMessageComposer> _composers;

        public ValidationResultFactory(IReadOnlyCollection<IMessageComposer> composers, EntityReferenceParser entityReferenceParser)
        {
            _entityReferenceParser = entityReferenceParser;
            _composers = composers.ToDictionary(x => x.MessageType, x => x);
        }

        public IReadOnlyCollection<ValidationResult> GetValidationResult(IReadOnlyCollection<Message> messages)
        {
            var messagesWithReferences = _entityReferenceParser.ParseEntityReferences(messages);
            var result = messagesWithReferences.Select(x => Compose(x.Message, x.References)).ToList();
            return result;
        }

        private ValidationResult Compose(Message message, IReadOnlyCollection<EntityReference> references)
        {
            try
            {
                IMessageComposer composer;
                if (!_composers.TryGetValue(message.MessageType, out composer))
                {
                    throw new ArgumentException($"Не найден сериализатор '{message.MessageType}'", nameof(message));
                }

                var composerResult = composer.Compose(message, references);

                return new ValidationResult
                {
                    MainReference = composerResult.MainReference,
                    References = composerResult.References,
                    Template = composerResult.Template,
                    Result = message.Result,
                    Rule = message.MessageType,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сериализации сообщения {message.MessageType}", ex);
            }
        }
    }
}