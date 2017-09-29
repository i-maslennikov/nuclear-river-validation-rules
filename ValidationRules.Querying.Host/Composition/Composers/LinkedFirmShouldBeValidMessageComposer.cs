using System;
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
        public MessageTypeCode MessageType => MessageTypeCode.LinkedFirmShouldBeValid;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var firmReference = references.Get<EntityTypeFirm>();
            var firmState = extra.ReadFirmState();

            return new MessageComposerResult(
                orderReference,
                GetFormat(firmState),
                firmReference);
        }

        public IEnumerable<Message> Distinct(IEnumerable<Message> messages)
        {
            // todo: Пересмотреть основной объект привязки, сделать фирму.
            // Сейчас объект привязки - заказ, но Erm при массовой проверке выводит только первое сообщение для фирмы (даже если заказов несколько).
            // Этот distinct сделан только для соответствия поведению erm, от него можно будет отказаться.
            return messages.GroupBy(x => x.References.Get<EntityTypeFirm>().Id, x => x).Select(x => x.OrderBy(y => y.OrderId).First());
        }

        private static string GetFormat(InvalidFirmState firmState)
        {
            switch (firmState)
            {
                case InvalidFirmState.Deleted:
                    return Resources.LinkedFirmShouldBeValid_Deleted;
                case InvalidFirmState.ClosedForever:
                    return Resources.LinkedFirmShouldBeValid_ClosedForever;
                case InvalidFirmState.ClosedForAscertainment:
                    return Resources.LinkedFirmShouldBeValid_ClosedForAscertainment;
                default:
                    throw new Exception(nameof(firmState));
            }
        }
    }
}