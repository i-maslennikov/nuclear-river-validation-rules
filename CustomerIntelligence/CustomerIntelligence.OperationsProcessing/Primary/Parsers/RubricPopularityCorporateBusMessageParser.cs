using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Replication.DTO;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers
{
    public sealed class RubricPopularityCorporateBusMessageParser : ICorporateBusMessageParser<RubricPopularity>
    {
        public bool TryParse(XElement xml, out RubricPopularity dto)
        {
            var branchElement = xml.Element("Branch");
            if (branchElement == null)
            {
                dto = null;
                return false;
            }

            try
            {
                dto = new RubricPopularity
                          {
                              ProjectId = (long)branchElement.Attribute("Code"),
                              Categories = xml.Descendants("Rubric")
                                              .Select(x =>
                                                          {
                                                              var advFirmCountAttr = x.Attribute("AdvFirmCount");
                                                              if (advFirmCountAttr == null)
                                                              {
                                                                  throw new ArgumentException();
                                                              }

                                                              return new RubricPopularity.Category
                                                                         {
                                                                             CategoryId = (long)x.Attribute("Code"),
                                                                             AdvertisersCount = (long)advFirmCountAttr
                                                                         };
                                                          })
                                              .ToArray()
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