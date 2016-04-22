using System;

namespace NuClear.Replication.Core.API.Commands
{
    public abstract class DeleteDataObjectCommandBase : IDataObjectCommand
    {
        public abstract Type DataObjectType { get; }
    }
}