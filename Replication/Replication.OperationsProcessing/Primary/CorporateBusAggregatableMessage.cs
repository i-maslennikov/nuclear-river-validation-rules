using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;
using NuClear.Replication.Core.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class CorporateBusAggregatableMessage : MessageBase, IAggregatableMessage
    {
        public override Guid Id => Guid.Empty;
        public IMessageFlow TargetFlow { get; set; }

        public IReadOnlyCollection<ICommand> Commands { get; set; }
    }
}