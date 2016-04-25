using System;

using NuClear.Replication.Core;

namespace NuClear.CustomerIntelligence.Replication.Commands
{
    public interface IAggregateCommand : ICommand
    {
        Type AggregateRootType { get; }
        long AggregateRootId { get; }
    }
}