using System;

namespace NuClear.Replication.Core.Commands
{
    public abstract class DeleteDataObjectCommandBase : ICommand
    {
        public abstract Type DataObjectType { get; }
    }
}