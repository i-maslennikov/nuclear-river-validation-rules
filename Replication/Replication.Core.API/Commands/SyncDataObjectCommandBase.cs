using System;

namespace NuClear.Replication.Core.API.Commands
{
    public abstract class SyncDataObjectCommandBase : ICommand
    {
        public abstract Type DataObjectType { get; }
    }
}