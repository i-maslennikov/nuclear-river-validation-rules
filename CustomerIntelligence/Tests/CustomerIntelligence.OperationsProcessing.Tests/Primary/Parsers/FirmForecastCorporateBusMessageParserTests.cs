using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers;
using NuClear.CustomerIntelligence.OperationsProcessing.Tests.Properties;
using NuClear.CustomerIntelligence.Replication.DTO;

using NUnit.Framework;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Tests.Primary.Parsers
{
    [TestFixture]
    public class FirmForecastCorporateBusMessageParserTests
    {
        [Test]
        public void ParseMessage()
        {
            var x = XElement.Parse(Resources.flowStatistics_FirmForecast_xml);
            var parser = new FirmForecastCorporateBusMessageParser();

            FirmForecast firmForecast;
            var result = parser.TryParse(x, out firmForecast);

            Assert.That(result, Is.True);
            Assert.That(firmForecast, Is.InstanceOf(typeof(FirmForecast)));

            Assert.That(firmForecast.ProjectId, Is.EqualTo(1));
            Assert.That(firmForecast.Firms.Count, Is.EqualTo(2));
            Assert.That(firmForecast.Firms.First().Id, Is.EqualTo(141274359267368));
            Assert.That(firmForecast.Firms.First().ForecastClick, Is.EqualTo(45));
            Assert.That(firmForecast.Firms.First().ForecastAmount, Is.EqualTo(670.00));
            Assert.That(firmForecast.Firms.First().Categories.Count, Is.EqualTo(2));
            Assert.That(firmForecast.Firms.First().Categories.First().Id, Is.EqualTo(110365));
            Assert.That(firmForecast.Firms.First().Categories.First().ForecastClick, Is.EqualTo(23));
            Assert.That(firmForecast.Firms.First().Categories.First().ForecastAmount, Is.EqualTo(230.00));
        }
    }
}
