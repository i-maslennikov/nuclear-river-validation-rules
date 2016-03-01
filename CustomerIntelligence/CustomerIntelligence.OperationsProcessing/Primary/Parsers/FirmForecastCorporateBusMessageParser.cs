using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers
{
    public sealed class FirmForecastCorporateBusMessageParser : ICorporateBusMessageParser
    {
        public bool TryParse(XElement xml, out IDataTransferObject dto)
        {
            try
            {
                dto = new FirmForecastDto
                    {
                        ProjectId = (long)xml.Attribute("BranchCode"),
                        Firms = ParseFirms(xml.Descendants("Firm"))
                    };
                return true;
            }
            catch (Exception)
            {
                dto = null;
                return false;
            }
        }

        private IReadOnlyCollection<FirmForecastDto.FirmDto> ParseFirms(IEnumerable<XElement> firms)
        {
            return firms.Select(firm => new FirmForecastDto.FirmDto
                {
                    Id = (long)firm.Attribute("Code"),
                    ForecastClick = (int)firm.Attribute("ForecastClick"),
                    ForecastAmount = (decimal)firm.Attribute("ForecastAmount"),
                    Categories = ParseCategories(firm.Descendants("Rubric"))
                }).ToArray();
        }

        private IReadOnlyCollection<FirmForecastDto.CategoryDto> ParseCategories(IEnumerable<XElement> categories)
        {
            return categories.Select(category => new FirmForecastDto.CategoryDto
                {
                    Id = (long)category.Attribute("Code"),
                    ForecastClick = (int)category.Attribute("ForecastClick"),
                    ForecastAmount = (decimal)category.Attribute("ForecastAmount"),
                }).ToArray();
        }
    }
}