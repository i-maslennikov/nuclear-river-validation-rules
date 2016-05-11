using System;

namespace NuClear.Replication.Core.Commands
{
    public interface ISyncDataObjectCommand : ICommand
    {
        Type DataObjectType { get; }
    }
}