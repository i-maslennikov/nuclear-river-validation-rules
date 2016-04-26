using System;

namespace NuClear.Replication.Core.Commands
{
    public interface ICreateDataObjectCommand : ICommand
    {
        Type DataObjectType { get; }
    }
}