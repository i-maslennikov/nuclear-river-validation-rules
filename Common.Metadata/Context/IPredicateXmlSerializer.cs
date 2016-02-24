using System.Xml.Linq;

namespace NuClear.River.Common.Metadata.Context
{
    public interface IPredicateXmlSerializer
    {
        XElement Serialize(Predicate predicate);
        Predicate Deserialize(XElement element);
    }
}