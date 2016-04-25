using System;

namespace NuClear.Replication.Core.Commands
{
    public abstract class CreateDataObjectCommandBase : ICommand
    {
        public abstract Type DataObjectType { get; }
    }
}