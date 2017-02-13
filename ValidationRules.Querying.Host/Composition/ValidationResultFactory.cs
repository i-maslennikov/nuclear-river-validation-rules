using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public sealed class ValidationResultFactory
    {
        private static readonly IDistinctor Default = new DefaultDistinctor();

        private readonly IReadOnlyDictionary<MessageTypeCode, IMessageComposer> _composers;
        private readonly IReadOnlyDictionary<MessageTypeCode, IDistinctor> _distinctors;
        private readonly EntityReferenceParser _entityReferenceParser;

        public ValidationResultFactory(IReadOnlyCollection<IMessageComposer> composers, IReadOnlyCollection<IDistinctor> distinctors, EntityReferenceParser entityReferenceParser)
        {
            _entityReferenceParser = entityReferenceParser;
            _composers = composers.ToDictionary(x => x.MessageType, x => x);
            _distinctors = distinctors.ToDictionary(x => x.MessageType, x => x);
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

        private IEnumerable<Message> MakeDistinct(IEnumerable<Message> messages)
            => messages.GroupBy(x => x.MessageType)
                       .SelectMany(x => DistinctorForMessageType(x.Key).Distinct(x));

        private IDistinctor DistinctorForMessageType(MessageTypeCode messageType)
        {
            IDistinctor distinctor;
            return _distinctors.TryGetValue(messageType, out distinctor) ? distinctor : Default;
        }

        private sealed class DefaultDistinctor : IDistinctor, IEqualityComparer<Message>
        {
            public MessageTypeCode MessageType => 0;

            public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
                => messages.GroupBy(x => new { x.OrderId, x.ProjectId })
                          .SelectMany(x => x.Distinct(this));

            bool IEqualityComparer<Message>.Equals(Message x, Message y)
                => XNode.EqualityComparer.Equals(x.Xml, y.Xml);

            int IEqualityComparer<Message>.GetHashCode(Message obj)
                => XNode.EqualityComparer.GetHashCode(obj.Xml);
        }
    }
}