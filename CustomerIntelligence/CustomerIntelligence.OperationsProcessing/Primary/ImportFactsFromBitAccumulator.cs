using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows;
using NuClear.CustomerIntelligence.OperationsProcessing.Primary.Parsers;
using NuClear.CustomerIntelligence.Replication.Commands;
using NuClear.CustomerIntelligence.Replication.DTO;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Replication.Core;
using NuClear.Replication.OperationsProcessing.Primary;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Tracing.API;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Primary
{
    public sealed class ImportFactsFromBitAccumulator :
        MessageProcessingContextAccumulatorBase<ImportFactsFromBitFlow, CorporateBusPerformedOperationsMessage, CorporateBusAggregatableMessage>
    {
        private readonly ITracer _tracer;

        public ImportFactsFromBitAccumulator(ITracer tracer)
        {
            _tracer = tracer;
        }

        protected override CorporateBusAggregatableMessage Process(CorporateBusPerformedOperationsMessage message)
        {
            var xmls = message.CorporateBusPackage.ConvertToXElements();

            var commands = xmls.SelectMany(CreateCommands)
                               .ToArray();

            return new CorporateBusAggregatableMessage
            {
                TargetFlow = MessageFlow,
                Commands = commands
            };
        }

        private IReadOnlyCollection<ICommand> CreateCommands(XElement xml)
        {
            var commands = new List<ICommand>();

            var corporateBusObjectName = xml.Name.LocalName.ToLowerInvariant();
            switch (corporateBusObjectName)
            {
                case "firmpopularity":
                    FirmPopularity firmPopularity;
                    if (TryParse<FirmPopularityCorporateBusMessageParser, FirmPopularity>(xml, out firmPopularity))
                    {
                        commands.Add(new ReplaceFirmPopularityCommand(firmPopularity));
                    }

                    break;

                case "rubricpopularity":
                    RubricPopularity rubricPopularity;
                    if (TryParse<RubricPopularityCorporateBusMessageParser, RubricPopularity>(xml, out rubricPopularity))
                    {
                        commands.Add(new ReplaceRubricPopularityCommand(rubricPopularity));
                    }

                    break;

                case "firmforecast":
                    FirmForecast firmForecast;
                    if (TryParse<FirmForecastCorporateBusMessageParser, FirmForecast>(xml, out firmForecast))
                    {
                        commands.Add(new ReplaceFirmForecastCommand(firmForecast));
                        commands.Add(new ReplaceFirmCategoryForecastCommand(firmForecast));
                    }

                    break;
            }

            return commands;
        }

        private bool TryParse<TParser, TResult>(XElement xml, out TResult result)
            where TResult : class
            where TParser : ICorporateBusMessageParser<TResult>, new()
        {
            var parser = new TParser();
            if (!parser.TryParse(xml, out result))
            {
                _tracer.Warn($"Skip {xml.Name.LocalName} message due to unsupported format");
                result = null;
                return false;
            }

            return true;
        }
    }
}