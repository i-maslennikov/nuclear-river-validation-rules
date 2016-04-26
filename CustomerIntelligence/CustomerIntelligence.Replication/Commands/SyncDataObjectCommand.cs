using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public sealed class SyncDataObjectCommand : ISyncDataObjectCommand
    {
        public SyncDataObjectCommand(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public Type DataObjectType { get; }
        public long DataObjectId { get; }
    }
}