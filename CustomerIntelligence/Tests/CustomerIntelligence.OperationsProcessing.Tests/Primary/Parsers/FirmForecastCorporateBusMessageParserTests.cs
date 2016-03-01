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
    public class FirmForecastCorporateBusMessageParserTests
    {
        [Test]
        public void ParseMessage()
        {
            var x = XElement.Parse(Resources.flowStatistics_FirmForecast_xml);
            var parser = new FirmForecastCorporateBusMessageParser();

            IDataTransferObject dto;
            var result = parser.TryParse(x, out dto);

            Assert.That(result, Is.True);
            Assert.That(dto, Is.InstanceOf(typeof(FirmForecastDto)));

            var ffdto = (FirmForecastDto)dto;
            Assert.That(ffdto.ProjectId, Is.EqualTo(1));
            Assert.That(ffdto.Firms.Count, Is.EqualTo(2));
            Assert.That(ffdto.Firms.First().Id, Is.EqualTo(141274359267368));
            Assert.That(ffdto.Firms.First().ForecastClick, Is.EqualTo(45));
            Assert.That(ffdto.Firms.First().ForecastAmount, Is.EqualTo(670.00));
            Assert.That(ffdto.Firms.First().Categories.Count, Is.EqualTo(2));
            Assert.That(ffdto.Firms.First().Categories.First().Id, Is.EqualTo(110365));
            Assert.That(ffdto.Firms.First().Categories.First().ForecastClick, Is.EqualTo(23));
            Assert.That(ffdto.Firms.First().Categories.First().ForecastAmount, Is.EqualTo(230.00));
        }
    }
}
