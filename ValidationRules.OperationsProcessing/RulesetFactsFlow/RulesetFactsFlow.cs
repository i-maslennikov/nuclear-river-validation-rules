using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.RulesetFactsFlow
{
    public sealed class RulesetFactsFlow : MessageFlowBase<RulesetFactsFlow>
    {
        public override Guid Id => new Guid("4F04437A-2F10-4A37-BB49-03810346AE84");

        public override string Description => nameof(RulesetFactsFlow);
    }
}