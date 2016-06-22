using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;
using NuClear.Replication.Core;

namespace NuClear.Replication.OperationsProcessing
{
    public sealed class AggregatableMessage<TCommand> : MessageBase, IAggregatableMessage
        where TCommand : ICommand
    {
        public override Guid Id => Guid.Empty;

        public IMessageFlow TargetFlow { get; set; }

        public IReadOnlyCollection<TCommand> Commands { get; set; }
    }
}