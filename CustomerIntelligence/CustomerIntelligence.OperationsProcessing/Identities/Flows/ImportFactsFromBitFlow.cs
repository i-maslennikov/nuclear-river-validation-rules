using System;

using NuClear.Messaging.Transports.CorporateBus.Flows;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows
{
    public sealed class ImportFactsFromBitFlow : CorporateBusFlowBase<ImportFactsFromBitFlow>
    {
        public override Guid Id => new Guid("C555F76E-E6F6-44A2-8323-1A54BDA2AF7D");

        public override string FlowName => "flowStatistics";

        public override string Description => "Поток статистики от OLAP";
    }
}