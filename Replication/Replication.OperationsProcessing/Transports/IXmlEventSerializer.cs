using System.Xml.Linq;

using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing.Transports
{
    public interface IXmlEventSerializer
    {
        IEvent Deserialize(XElement message);
        XElement Serialize(IEvent @event);
    }
}
