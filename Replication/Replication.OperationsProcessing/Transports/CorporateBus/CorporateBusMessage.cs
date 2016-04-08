using System;

using NuClear.Messaging.API;
using NuClear.Messaging.Transports.CorporateBus.API;

namespace NuClear.Replication.OperationsProcessing.Transports.CorporateBus
{
    public sealed class CorporateBusPerformedOperationsMessage : MessageBase
    {
        public CorporateBusPerformedOperationsMessage(CorporateBusPackage package)
        {
            Id = Guid.NewGuid();
            CorporateBusPackage = package;
        }

        public override Guid Id { get; }

        public CorporateBusPackage CorporateBusPackage { get; }
    }
}