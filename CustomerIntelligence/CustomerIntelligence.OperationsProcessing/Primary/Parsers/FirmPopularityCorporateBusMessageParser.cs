using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers
{
    public sealed class FirmPopularityCorporateBusMessageParser : ICorporateBusMessageParser
    {
        public bool TryParse(XElement xml, out IDataTransferObject dto)
        {
            try
            {
                dto = new FirmStatisticsDto
                {
                    ProjectId = (long)xml.Attribute("BranchCode"),
                    Firms = xml.Descendants("Firm").Select(x =>
                    {
                        var firmDto = new FirmStatisticsDto.FirmDto
                        {
                            FirmId = (long)x.Attribute("Code"),
                            Categories = x.Descendants("Rubric").Select(y =>
                            {
                                var clickCountAttr = y.Attribute("ClickCount");
                                var impressionCountAttr = y.Attribute("ImpressionCount");
                                if (clickCountAttr == null || impressionCountAttr == null)
                                {
                                    throw new ArgumentException();
                                }

                                var rubricDto = new FirmStatisticsDto.FirmDto.CategoryDto
                                {
                                    CategoryId = (long)y.Attribute("Code"),
                                    Hits = (int)clickCountAttr,
                                    Shows = (int)impressionCountAttr,
                                };

                                return rubricDto;
                            }).ToArray()
                        };

                        return firmDto;
                    }).ToArray(),
                };

                return true;
            }
            catch (ArgumentException)
            {
                dto = null;
                return false;
            }
        }
    }
}