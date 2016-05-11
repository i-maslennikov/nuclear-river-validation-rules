using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Replication.DTO;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers
{
    public sealed class FirmForecastCorporateBusMessageParser : ICorporateBusMessageParser<FirmForecast>
    {
        public bool TryParse(XElement xml, out FirmForecast dto)
        {
            try
            {
                dto = new FirmForecast
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

        private IReadOnlyCollection<FirmForecast.Firm> ParseFirms(IEnumerable<XElement> firms)
        {
            return firms.Select(firm => new FirmForecast.Firm
                                            {
                                                Id = (long)firm.Attribute("Code"),
                                                ForecastClick = (int)firm.Attribute("ForecastClick"),
                                                ForecastAmount = (decimal)firm.Attribute("ForecastAmount"),
                                                Categories = ParseCategories(firm.Descendants("Rubric"))
                                            })
                        .ToArray();
        }

        private IReadOnlyCollection<FirmForecast.Category> ParseCategories(IEnumerable<XElement> categories)
        {
            return categories
                .Select(category => new FirmForecast.Category
                                        {
                                            Id = (long)category.Attribute("Code"),
                                            ForecastClick = (int)category.Attribute("ForecastClick"),
                                            ForecastAmount = (decimal)category.Attribute("ForecastAmount"),
                                        })
                .ToArray();
        }
    }
}