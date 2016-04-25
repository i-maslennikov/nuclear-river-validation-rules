using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public sealed class SyncDataObjectCommand : SyncDataObjectCommandBase
    {
        public SyncDataObjectCommand(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public override Type DataObjectType { get; }
        public long DataObjectId { get; }
    }
}