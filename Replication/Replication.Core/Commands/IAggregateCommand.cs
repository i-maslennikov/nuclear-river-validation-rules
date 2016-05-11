using System;

namespace NuClear.Replication.Core.Commands
{
    public interface IAggregateCommand : ICommand
    {
        Type AggregateRootType { get; }
    }
}