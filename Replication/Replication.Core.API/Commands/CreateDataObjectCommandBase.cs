using System;

namespace NuClear.Replication.Core.API.Commands
{
    public abstract class CreateDataObjectCommandBase : IDataObjectCommand
    {
        public abstract Type DataObjectType { get; }
    }
}