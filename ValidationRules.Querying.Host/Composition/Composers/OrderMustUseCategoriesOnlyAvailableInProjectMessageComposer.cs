using System.Collections.Generic;
using System.Linq;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderMustUseCategoriesOnlyAvailableInProjectMessageComposer : IMessageComposer, IDistinctor
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderMustUseCategoriesOnlyAvailableInProject;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            // todo: сортировки нет в требованиях, сделана для соответствия erm
            var categoryReferences = references.GetMany<EntityTypeCategory>().OrderBy(x => x.Name);

            var templateWithVariableParameterCount =
                Resources.OrderMustUseCategoriesOnlyAvailableInProject + string.Join(", ", categoryReferences.Select((x, i) => "{" + (i + 1) + "}"));

            return new MessageComposerResult(
                orderReference,
                templateWithVariableParameterCount,
                new []{ orderPositionReference }.Concat(categoryReferences).ToArray());
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
        {
            return messages.GroupBy(x =>
            {
                var opaReference = x.References.Get<EntityTypeOrderPositionAdvertisement>();

                return new
                {
                    x.OrderId,
                    x.MessageType,
                    x.ProjectId,
                    OrderPositionId = opaReference.Children.Get<EntityTypeOrderPosition>().Id,
                    PositionId = opaReference.Children.Get<EntityTypePosition>().Id
                };
            }, x => x.References)
            .Select(x => new Message
            {
                OrderId = x.Key.OrderId,
                MessageType = x.Key.MessageType,
                ProjectId = x.Key.ProjectId,
                References = new List<Reference>(x.SelectMany(ReferenceExtensions.GetMany<EntityTypeCategory>))
                {
                    new Reference(EntityTypeOrder.Instance.Id, x.Key.OrderId.Value),
                    new Reference(EntityTypeOrderPositionAdvertisement.Instance.Id, 0,
                                    new Reference(EntityTypeOrderPosition.Instance.Id, x.Key.OrderPositionId),
                                    new Reference(EntityTypePosition.Instance.Id, x.Key.PositionId))
                }
            });
        }
    }
}