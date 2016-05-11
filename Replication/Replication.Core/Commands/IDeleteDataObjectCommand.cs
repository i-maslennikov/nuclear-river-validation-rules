using System;

namespace NuClear.Replication.Core.Commands
{
    public interface IDeleteDataObjectCommand : ICommand
    {
        Type DataObjectType { get; }
    }
}