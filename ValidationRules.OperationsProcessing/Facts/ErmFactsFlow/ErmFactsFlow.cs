using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.Facts.ErmFactsFlow
{
    public sealed class ErmFactsFlow : MessageFlowBase<ErmFactsFlow>
    {
        public override Guid Id => new Guid("6A75B8B4-74A6-4523-9388-84E4DFFD5B06");

        public override string Description => nameof(ErmFactsFlow);
    }
}