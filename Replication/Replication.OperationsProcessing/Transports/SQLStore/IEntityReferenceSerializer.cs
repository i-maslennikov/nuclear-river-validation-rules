using System;
using System.Xml.Linq;

using NuClear.River.Common.Metadata.Model.Operations;

namespace NuClear.Replication.OperationsProcessing.Transports.SQLStore
{
    [Obsolete]
    public interface IEntityReferenceSerializer
    {
        EntityReference Deserialize(XElement element);
        XElement Serialize(string name, EntityReference reference);
    }
}