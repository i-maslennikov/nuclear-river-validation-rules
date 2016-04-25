using System.Xml.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers;
using NuClear.CustomerIntelligence.OperationsProcessing.Tests.Properties;
using NuClear.CustomerIntelligence.Replication.DTO;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.Primary.Parsers
{
    [TestFixture]
    public class FirmPopularityCorporateBusMessageParserTests
    {
        [Test]
        public void ParseMessage()
        {
            var x = XElement.Parse(Resources.flowStatistics_FirmPopularity_xml);
            var parser = new FirmPopularityCorporateBusMessageParser();

            FirmPopularity firmPopularity;
            var result = parser.TryParse(x, out firmPopularity);

            Assert.That(result, Is.True);
        }
    }
}
