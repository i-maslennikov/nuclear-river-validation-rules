using System;

namespace NuClear.Replication.Core.Commands
{
    public abstract class SyncDataObjectCommandBase : ICommand
    {
        public abstract Type DataObjectType { get; }
    }
}