using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers
{
    public sealed class RubricPopularityCorporateBusMessageParser : ICorporateBusMessageParser
    {
        public bool TryParse(XElement xml, out IDataTransferObject dto)
        {
            var branchElement = xml.Element("Branch");
            if (branchElement == null)
            {
                dto = null;
                return false;
            }

            try
            {
                dto = new CategoryStatisticsDto
                {
                    ProjectId = (long)branchElement.Attribute("Code"),
                    Categories = xml.Descendants("Rubric").Select(x =>
                    {
                        var advFirmCountAttr = x.Attribute("AdvFirmCount");
                        if (advFirmCountAttr == null)
                        {
                            throw new ArgumentException();
                        }

                        var rubricDto = new CategoryStatisticsDto.CategoryDto
                        {
                            CategoryId = (long)x.Attribute("Code"),
                            AdvertisersCount = (long)advFirmCountAttr
                        };

                        return rubricDto;
                    }).ToArray()
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