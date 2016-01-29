using System.Linq;
using System.Xml.Linq;

namespace NuClear.AdvancedSearch.Common.Metadata.Context
{
    public sealed class PredicateXmlSerializer : IPredicateXmlSerializer
    {
        public XElement Serialize(Predicate predicate)
        {
            return new XElement("predicate",
                                predicate.Properties.Select(prop => new XAttribute(prop.Key, prop.Value)),
                                predicate.Childs.Select(Serialize));
        }

        public Predicate Deserialize(XElement element)
        {
            return new Predicate(element.Attributes().ToDictionary(a => a.Name.LocalName, a => a.Value),
                                 element.Elements().Select(Deserialize).ToArray());
        }
    }
}