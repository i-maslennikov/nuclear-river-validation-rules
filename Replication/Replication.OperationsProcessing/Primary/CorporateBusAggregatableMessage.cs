using System;
using System.Collections.Generic;

using NuClear.Messaging.API;
using NuClear.Messaging.API.Flows;
using NuClear.Messaging.API.Processing;
using NuClear.River.Common.Metadata.Model;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    public sealed class CorporateBusAggregatableMessage : MessageBase, IAggregatableMessage
    {
        public override Guid Id { get; } = Guid.Empty;
        public IMessageFlow TargetFlow { get; set; }

        public IReadOnlyCollection<IDataTransferObject> Dtos { get; set; }
    }
}