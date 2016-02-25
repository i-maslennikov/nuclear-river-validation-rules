using System;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.River.Common.Metadata.Model;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromBitFlow, CorporateBusPerformedOperationsMessage, CorporateBusAggregatableMessage>
    {
        private readonly ITracer _tracer;

        public ImportFactsFromBitAccumulator(ITracer tracer)
        {
            _tracer = tracer;
        }

        protected override CorporateBusAggregatableMessage Process(CorporateBusPerformedOperationsMessage message)
        {
            var xmls = message.Packages.SelectMany(x => x.ConvertToXElements());

            var dtos = xmls.Select(x =>
                                   {
                                       IDataTransferObject dto;
                                       var parsed = TryParseXml(x, out dto);
                                       return Tuple.Create(parsed, dto);
                                   })
                           .Where(x => x.Item1)
                           .Select(x => x.Item2)
                           .ToArray();

            return new CorporateBusAggregatableMessage
            {
                TargetFlow = MessageFlow,
                Dtos = dtos,
            };
        }

        private bool TryParseXml(XElement xml, out IDataTransferObject dto)
        {
            switch (xml.Name.LocalName.ToLowerInvariant())
            {
                case "firmpopularity":
                    return TryParseFirmPopularity(xml, out dto);
                case "rubricpopularity":
                    return TryParseRubricPopularity(xml, out dto);
                default:
                    dto = null;
                    return false;
            }
        }

        private bool TryParseFirmPopularity(XElement xml, out IDataTransferObject dto)
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
                _tracer.Warn("Skip FirmPopularity message due to unsupported format");
                dto = null;
                return false;
            }
        }

        private bool TryParseRubricPopularity(XElement xml, out IDataTransferObject dto)
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
                _tracer.Warn("Skip RubricPopularity message due to unsupported format");
                dto = null;
                return false;
            }
        }
    }
}