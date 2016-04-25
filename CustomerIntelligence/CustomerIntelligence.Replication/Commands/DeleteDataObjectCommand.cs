using System;

using NuClear.Replication.Core.Commands;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public sealed class DeleteDataObjectCommand : DeleteDataObjectCommandBase
    {
        public DeleteDataObjectCommand(Type dataObjectType, long dataObjectId)
        {
            DataObjectType = dataObjectType;
            DataObjectId = dataObjectId;
        }

        public override Type DataObjectType { get; }
        public long DataObjectId { get; }
    }
}