using System;

namespace NuClear.Replication.Core.API.Commands
{
    public abstract class SyncDataObjectCommandBase : IDataObjectCommand
    {
        public abstract Type DataObjectType { get; }
    }
}