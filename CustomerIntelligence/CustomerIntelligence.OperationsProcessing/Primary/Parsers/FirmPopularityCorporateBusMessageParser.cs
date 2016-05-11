using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Replication.DTO;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers
{
    public sealed class FirmPopularityCorporateBusMessageParser : ICorporateBusMessageParser<FirmPopularity>
    {
        public bool TryParse(XElement xml, out FirmPopularity dto)
        {
            try
            {
                dto = new FirmPopularity
                          {
                              ProjectId = (long)xml.Attribute("BranchCode"),
                              Firms = xml.Descendants("Firm")
                                         .Select(x =>
                                                     {
                                                         var firmDto = new FirmPopularity.Firm
                                                                           {
                                                                               FirmId = (long)x.Attribute("Code"),
                                                                               Categories = x.Descendants("Rubric")
                                                                                             .Select(y =>
                                                                                                         {
                                                                                                             var clickCountAttr = y.Attribute("ClickCount");
                                                                                                             var impressionCountAttr = y.Attribute("ImpressionCount");
                                                                                                             if (clickCountAttr == null || impressionCountAttr == null)
                                                                                                             {
                                                                                                                 throw new ArgumentException();
                                                                                                             }

                                                                                                             return new FirmPopularity.Firm.Category
                                                                                                                        {
                                                                                                                            CategoryId = (long)y.Attribute("Code"),
                                                                                                                            Hits = (int)clickCountAttr,
                                                                                                                            Shows = (int)impressionCountAttr,
                                                                                                                        };
                                                                                                         })
                                                                                             .ToArray()
                                                                           };

                                                         return firmDto;
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