using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.OperationsProcessing
{
    public sealed class OperationAggregatableMessage<TOperation> : MessageBase, IAggregatableMessage
        where TOperation : IOperation
    {
        public override Guid Id => Guid.Empty;

        public IMessageFlow TargetFlow { get; set; }

        public IReadOnlyCollection<TOperation> Operations { get; set; }

        public DateTime OperationTime { get; set; }
    }
}