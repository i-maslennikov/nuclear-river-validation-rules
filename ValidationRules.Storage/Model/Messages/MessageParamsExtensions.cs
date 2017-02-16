using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NuClear.ValidationRules.Storage.Model.Messages
{
    public static class MessageParamsExtensions
    {
        public static XDocument ToXDocument(this MessageParams messageParams)
            => new XDocument(new XElement("root", messageParams.ExtraParameters.Select(ToAttribute).Concat(messageParams.Children.Select(ToElement).ToArray())));

        private static object ToAttribute(KeyValuePair<string, object> pair)
            => new XAttribute(pair.Key, pair.Value);

        private static object ToElement(this Reference reference)
            => new XElement("ref", new XAttribute("type", reference.EntityType), new XAttribute("id", reference.Id), reference.Children.Select(x => x.ToElement()));
    }
}