using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common.Entities;
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
        private readonly IReadOnlyDictionary<int, string> _knownEntityTypes;
        private readonly NameResolvingService _nameResolvingService;

        public ValidationResultFactory(
            IReadOnlyCollection<IMessageComposer> composers,
            IReadOnlyCollection<IDistinctor> distinctors,
            IReadOnlyCollection<IEntityType> knownEntityTypes,
            NameResolvingService nameResolvingService)
        {
            _knownEntityTypes = knownEntityTypes.ToDictionary(x => x.Id, x => x.Description);
            _nameResolvingService = nameResolvingService;
            _composers = composers.ToDictionary(x => x.MessageType, x => x);
            _distinctors = distinctors.ToDictionary(x => x.MessageType, x => x);
        }

        public IReadOnlyCollection<ValidationResult> GetValidationResult(IReadOnlyCollection<Message> messages)
        {
            var resolvedNames = _nameResolvingService.Resolve(messages);
            var result = MakeDistinct(messages).Select(x => Compose(x, resolvedNames)).ToList();
            return result;
        }

        private ValidationResult Compose(Message message, ResolvedNameContainer resolvedNames)
        {
            try
            {
                IMessageComposer composer;
                if (!_composers.TryGetValue(message.MessageType, out composer))
                {
                    throw new ArgumentException($"Не найден сериализатор '{message.MessageType}'", nameof(message));
                }

                var composerResult = composer.Compose(message.References.Select(resolvedNames.For).ToArray(), message.Extra);

                return new ValidationResult
                {
                    MainReference = ConvertReference(composerResult.MainReference),
                    References = composerResult.References.Select(ConvertReference).ToList(),
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

        private ValidationResult.Reference ConvertReference(NamedReference reference)
            => new ValidationResult.Reference { Id = reference.Reference.Id, Name = reference.Name, Type = _knownEntityTypes[reference.Reference.EntityType] };

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
                => x.References.Count == y.References.Count
                   && x.References.Zip(y.References, (l, r) => Reference.Comparer.Equals(l, r)).All(equal => equal);

            int IEqualityComparer<Message>.GetHashCode(Message obj)
                => obj.References.Aggregate(0, (accum, reference) => (accum * 367) ^ Reference.Comparer.GetHashCode(reference));
        }
    }
}