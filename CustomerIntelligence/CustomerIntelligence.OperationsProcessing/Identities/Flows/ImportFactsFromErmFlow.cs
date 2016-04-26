using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows
{
    public sealed class ImportFactsFromErmFlow : MessageFlowBase<ImportFactsFromErmFlow>
    {
        public override Guid Id => new Guid("9F2C5A2A-924C-485A-9790-9066631DB307");

        public override string Description => "Маркер для потока выполненных операций в системе обеспечивающих репликацию изменений в домен Customer Intelligence";
    }
}