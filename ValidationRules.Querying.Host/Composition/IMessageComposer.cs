using System;
using System.Collections.Generic;
using System.Linq;

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

        public MessageComposerResult(NamedReference mainReference, string template, params object[] args)
        {
            var index = 0;
            var references = args.OfType<NamedReference>().ToArray();
            var templateParams = args.Select(x => PrepareTemplateParameter(x, ref index)).ToArray();

            MainReference = mainReference;
            Template = string.Format(template, templateParams);
            References = references;
        }

        public MessageComposerResult(NamedReference mainReference, string template)
            :this(mainReference, template, Array.Empty<NamedReference>())
        {
        }

        public NamedReference MainReference { get; set; }
        public string Template { get; set; }
        public NamedReference[] References { get; set; }

        private static object PrepareTemplateParameter(object p, ref int index)
        {
            switch (p)
            {
                case string str:
                    return str.Replace("{", "{{").Replace("}", "}}");
                case NamedReference reference:
                    return $"{{{index++}}}";
                default:
                    return p;
            }
        }
    }
}