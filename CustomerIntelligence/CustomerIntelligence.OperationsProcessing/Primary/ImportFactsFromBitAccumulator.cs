using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers;
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

        private readonly IDictionary<string, ICorporateBusMessageParser> _parsers =
            new Dictionary<string, ICorporateBusMessageParser>
                {
                    { "firmpopularity", new FirmPopularityCorporateBusMessageParser() },
                    { "rubricpopularity", new RubricPopularityCorporateBusMessageParser() },
                    { "firmforecast", new FirmForecastCorporateBusMessageParser() },
                };

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
            var corporateBusObjectName = xml.Name.LocalName.ToLowerInvariant();
            ICorporateBusMessageParser parser;
            if (!_parsers.TryGetValue(corporateBusObjectName, out parser))
            {
                    dto = null;
                    return false;
            }

            if (!parser.TryParse(xml, out dto))
        {
                _tracer.Warn($"Skip {corporateBusObjectName} message due to unsupported format");
                dto = null;
                return false;
            }

                return true;
            }
    }
}