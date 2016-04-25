using System;

namespace NuClear.Replication.Core.API.Commands
{
    public abstract class CreateDataObjectCommandBase : ICommand
    {
        public abstract Type DataObjectType { get; }
    }
}