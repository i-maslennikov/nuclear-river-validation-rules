using System;

namespace NuClear.Replication.Core.Commands
{
    public interface IReplaceDataObjectCommand : ICommand
    {
        Type DataObjectType { get; }
    }
}