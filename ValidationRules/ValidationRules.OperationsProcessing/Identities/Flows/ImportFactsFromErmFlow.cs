using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Identities.Flows
{
    public sealed class ImportFactsFromErmFlow : MessageFlowBase<ImportFactsFromErmFlow>
    {
        public override Guid Id => new Guid("6A75B8B4-74A6-4523-9388-84E4DFFD5B06");

        public override string Description => "Маркер для потока выполненных операций в системе обеспечивающих репликацию изменений в домен Validation Rules";
    }
}