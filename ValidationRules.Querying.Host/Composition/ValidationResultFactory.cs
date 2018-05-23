using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.Model.Common.Entities;
using NuClear.ValidationRules.Querying.Host.DataAccess;
using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

using Version = NuClear.ValidationRules.Storage.Model.Messages.Version;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public sealed class ValidationResultFactory
    {
        private static readonly IDistinctor Default = new DefaultDistinctor();

        private readonly IReadOnlyDictionary<MessageTypeCode, IMessageComposer> _composers;
        private readonly IReadOnlyDictionary<MessageTypeCode, IDistinctor> _distinctors;
        private readonly IMessageSeverityProvider _messageSeverityProvider;
        private readonly IReadOnlyDictionary<int, string> _knownEntityTypes;
        private readonly NameResolvingService _nameResolvingService;

        public ValidationResultFactory(
            IReadOnlyCollection<IMessageComposer> composers,
            IReadOnlyCollection<IDistinctor> distinctors,
            IMessageSeverityProvider messageSeverityProvider,
            IReadOnlyCollection<IEntityType> knownEntityTypes,
            NameResolvingService nameResolvingService)
        {
            _knownEntityTypes = knownEntityTypes.ToDictionary(x => x.Id, x => x.Description);
            _nameResolvingService = nameResolvingService;
            _messageSeverityProvider = messageSeverityProvider;
            _composers = composers.ToDictionary(x => x.MessageType);
            _distinctors = distinctors.ToDictionary(x => x.MessageType);
        }

        public IReadOnlyCollection<ValidationResult> GetValidationResult(IReadOnlyCollection<Version.ValidationResult> validationResults, ICheckModeDescriptor checkModeDescriptor)
        {
            var messages = validationResults.Select(ToMessage).ToList();
            var resolvedNames = _nameResolvingService.Resolve(messages);
            var result = MakeDistinct(messages).Select(x => Compose(x, resolvedNames, checkModeDescriptor)).ToList();
            return result;
        }

        private ValidationResult Compose(Message message, ResolvedNameContainer resolvedNames, ICheckModeDescriptor checkModeDescriptor)
        {
            try
            {
                if (!_composers.TryGetValue(message.MessageType, out var composer))
                {
                    throw new ArgumentException($"Не найден сериализатор '{message.MessageType}'", nameof(message));
                }

                var composerResult = composer.Compose(message.References.Select(resolvedNames.For).ToArray(), message.Extra);

                return new ValidationResult
                    {
                        MainReference = ConvertReference(composerResult.MainReference),
                        References = composerResult.References.Select(ConvertReference).ToList(),
                        Template = composerResult.Template,
                        Result = _messageSeverityProvider.GetLevel(message, checkModeDescriptor),
                        Rule = message.MessageType
                    };
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при сериализации сообщения {message.MessageType}", ex);
            }
        }

        private ValidationResult.Reference ConvertReference(NamedReference reference)
            => new ValidationResult.Reference { Id = reference.Reference.Id, Name = reference.Name, Type = _knownEntityTypes[reference.Reference.EntityType] };

        private IEnumerable<Message> MakeDistinct(IEnumerable<Message> messages)
            => messages.GroupBy(x => x.MessageType)
                       .SelectMany(x => DistinctorForMessageType(x.Key).Distinct(x));

        private IDistinctor DistinctorForMessageType(MessageTypeCode messageType)
        {
            return _distinctors.TryGetValue(messageType, out var distinctor) ? distinctor : Default;
        }

        private static Message ToMessage(Version.ValidationResult x)
            => new Message
                {
                    MessageType = (MessageTypeCode)x.MessageType,
                    References = ParseReferences(x.MessageParams),
                    Extra = ParseExtra(x.MessageParams),
                    OrderId = x.OrderId,
                    ProjectId = x.ProjectId,
                };

        private static IReadOnlyCollection<Reference> ParseReferences(XDocument messageParams)
            => messageParams.Root.Elements().Select(Parse).ToList();

        private static IReadOnlyDictionary<string, string> ParseExtra(XDocument messageParams)
            => messageParams.Root.Attributes().ToDictionary(x => x.Name.LocalName, x => x.Value);

        private static Reference Parse(XElement element)
            => new Reference((int)element.Attribute("type"), (long)element.Attribute("id"), element.Elements().Select(Parse).ToArray());

        private sealed class DefaultDistinctor : IDistinctor, IEqualityComparer<Message>
        {
            public MessageTypeCode MessageType => 0;

            public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
                => messages.GroupBy(x => new { x.OrderId, x.ProjectId })
                           .SelectMany(x => x.Distinct(this));

            bool IEqualityComparer<Message>.Equals(Message x, Message y)
                => x.References.Count == y.References.Count
                   && x.References.SequenceEqual(y.References, Reference.Comparer)
                   && x.Extra.Count == y.Extra.Count
                   && x.Extra.All(pair => y.Extra.TryGetValue(pair.Key, out var value) && pair.Value == value);

            int IEqualityComparer<Message>.GetHashCode(Message obj)
                => obj.References.Aggregate(0, (accum, reference) => (accum * 397) ^ Reference.Comparer.GetHashCode(reference)) ^
                   obj.Extra.Aggregate(0, (accum, pair) => (accum * 397) ^ (pair.Key.GetHashCode() * 397) ^ pair.Value.GetHashCode());
        }
    }
}