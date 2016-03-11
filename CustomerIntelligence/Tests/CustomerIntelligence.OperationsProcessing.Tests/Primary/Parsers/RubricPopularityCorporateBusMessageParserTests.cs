using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers;
using NuClear.CustomerIntelligence.OperationsProcessing.Tests.Properties;
using NuClear.River.Common.Metadata.Model;

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

            IDataTransferObject dto;
            var result = parser.TryParse(x, out dto);

            Assert.That(result, Is.True);
            Assert.That(dto, Is.InstanceOf(typeof(CategoryStatisticsDto)));
        }
    }
}
