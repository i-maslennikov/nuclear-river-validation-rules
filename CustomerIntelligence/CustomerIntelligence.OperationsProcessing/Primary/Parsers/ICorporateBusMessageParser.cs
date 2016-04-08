using System.Xml.Linq;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers
{
    public interface ICorporateBusMessageParser<TResult>
    {
        bool TryParse(XElement xml, out TResult dto);
    }
}