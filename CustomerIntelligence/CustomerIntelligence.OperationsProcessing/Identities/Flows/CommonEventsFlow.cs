using System;

using NuClear.Messaging.API.Flows;

namespace NuClear.CustomerIntelligence.OperationsProcessing.Identities.Flows
{
    public sealed class CommonEventsFlow : MessageFlowBase<CommonEventsFlow>
    {
        public override Guid Id => new Guid("96F17B1A-4CC8-40CC-9A92-16D87733C39F");

        public override string Description => "";
    }
}