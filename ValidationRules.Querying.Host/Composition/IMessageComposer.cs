using System.Collections.Generic;

using NuClear.ValidationRules.Querying.Host.Model;
using NuClear.ValidationRules.Storage.Model.Messages;

namespace NuClear.ValidationRules.Querying.Host.Composition
{
    public interface IMessageComposer
    {
        MessageTypeCode MessageType { get; }
        MessageComposerResult Compose(NamedReference[] references, IReadOnlyDictionary<string, string> extra);
    }

    public class MessageComposerResult
    {
        public MessageComposerResult(NamedReference mainReference, string template, params NamedReference[] references)
        {
            MainReference = mainReference;
            Template = template;
            References = references;
        }

        public NamedReference MainReference { get; set; }
        public string Template { get; set; }
        public NamedReference[] References { get; set; }
    }
}