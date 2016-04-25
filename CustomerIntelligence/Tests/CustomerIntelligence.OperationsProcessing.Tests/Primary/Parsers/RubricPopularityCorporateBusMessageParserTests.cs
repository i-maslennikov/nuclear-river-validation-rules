using System.Xml.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers;
using NuClear.CustomerIntelligence.OperationsProcessing.Tests.Properties;
using NuClear.CustomerIntelligence.Replication.DTO;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.Primary.Parsers
{
    [TestFixture]
    public class RubricPopularityCorporateBusMessageParserTests
    {
        [Test]
        public void ParseMessage()
        {
            var x = XElement.Parse(Resources.flowStatistics_RubricPopularity_xml);
            var parser = new RubricPopularityCorporateBusMessageParser();

            RubricPopularity rubricPopularity;
            var result = parser.TryParse(x, out rubricPopularity);

            Assert.That(result, Is.True);
        }
    }
}
