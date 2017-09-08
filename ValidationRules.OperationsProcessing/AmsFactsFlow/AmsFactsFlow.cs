using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.ValidationRules.OperationsProcessing.AmsFactsFlow
{
    public sealed class AmsFactsFlow : MessageFlowBase<AmsFactsFlow>
    {
        public override Guid Id => new Guid("A2878E80-992A-4602-8FD6-B10AE85BBFFE");

        public override string Description => nameof(AmsFactsFlow);
    }
}