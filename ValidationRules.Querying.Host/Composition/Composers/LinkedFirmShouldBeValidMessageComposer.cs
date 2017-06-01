using System.Linq;
using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.FirmRules.Aggregates;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class LinkedFirmShouldBeValidMessageComposer : IMessageComposer, IDistinctor
    {
        private static readonly Dictionary<InvalidFirmState, string> Formats = new Dictionary<InvalidFirmState, string>
            {
                { InvalidFirmState.Deleted, Resources.FirmIsDeleted },
                { InvalidFirmState.ClosedForever, Resources.FirmIsPermanentlyClosed },
                { InvalidFirmState.ClosedForAscertainment, Resources.OrderFirmHiddenForAscertainmentTemplate }
            };

        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmShouldBeValid;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var firmReference = references.Get<EntityTypeFirm>();
            var firmState = extra.ReadFirmState();

            return new MessageComposerResult(
                orderReference,
                Formats[firmState],
                firmReference);
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
        {
            // todo: Пересмотреть основной объект привязки, сделать фирму.
            // Сейчас объект привязки - заказ, но Erm при массовой проверке выводит только первое сообщение для фирмы (даже если заказов несколько).
            // Этот distinct сделан только для соответствия поведению erm, от него можно будет отказаться.
            return messages.GroupBy(x => x.References.Get<EntityTypeFirm>().Id, x => x).Select(x => x.OrderBy(y => y.OrderId).First());
        }
    }
}