using System.Xml.Linq;

using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers
{
    public interface ICorporateBusMessageParser
    {
        bool TryParse(XElement xml, out IDataTransferObject dto);
    }
}