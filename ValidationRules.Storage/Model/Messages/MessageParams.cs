using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public class MessageParams
    {
        private static readonly IReadOnlyDictionary<string, object> Empty = new Dictionary<string, object>();

        public MessageParams(params Reference[] children)
            : this(Empty, children)
        {
        }

        public MessageParams(IReadOnlyDictionary<string, object> extraParameters, params Reference[] children)
        {
            ExtraParameters = extraParameters;
            Children = children;
        }

        public IReadOnlyDictionary<string, object> ExtraParameters { get; }
        public IReadOnlyCollection<Reference> Children { get; }

        public XDocument ToXDocument()
            => new XDocument(new XElement("root", ExtraParameters.Select(ToAttribute).Concat(Children.Select(ToElement).ToArray())));

        private static object ToAttribute(KeyValuePair<string, object> pair)
            => new XAttribute(pair.Key, pair.Value);

        private static object ToElement(Reference reference)
            => new XElement("ref", new XAttribute("type", reference.EntityType), new XAttribute("id", reference.Id), reference.Children.Select(ToElement));

    }
}