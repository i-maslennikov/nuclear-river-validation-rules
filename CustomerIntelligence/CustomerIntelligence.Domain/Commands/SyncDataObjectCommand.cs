using System;

namespace NuClear.CustomerIntelligence.Domain.Commands
{
    public sealed class SyncDataObjectCommand : IDataObjectCommand
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