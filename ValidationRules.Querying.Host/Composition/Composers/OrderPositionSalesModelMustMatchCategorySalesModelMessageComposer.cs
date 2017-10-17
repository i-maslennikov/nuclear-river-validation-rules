using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Properties;
using NuClear.ValidationRules.Storage.Identitites.EntityTypes;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition.Composers
{
    public sealed class OrderPositionSalesModelMustMatchCategorySalesModelMessageComposer : IMessageComposer
    {
        public MessageTypeCode MessageType => MessageTypeCode.OrderPositionSalesModelMustMatchCategorySalesModel;

        public MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra)
        {
            var orderReference = references.Get<EntityTypeOrder>();
            var orderPositionReference = references.Get<EntityTypeOrderPosition>();
            var categoryReference = references.Get<EntityTypeCategory>();
            var projectReference = references.Get<EntityTypeProject>();
            var begin = extra.ReadBeginDate();

            return new MessageComposerResult(
                orderReference,
                Resources.OrderPositionSalesModelMustMatchCategorySalesModel,
                orderPositionReference,
                categoryReference,
                projectReference,
                begin);
        }
    }
}